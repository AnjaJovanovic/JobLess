using FluentAssertions;
using JobLess.JobApplication.Application.Commands.ApplyForJob;
using JobLess.JobApplication.Application.Interfaces;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class ApplyForJobCommandHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly Mock<IClientProfileLookupService> _clientLookupMock;
    private readonly Mock<ICompanyLookupService> _companyLookupMock;
    private readonly ApplyForJobCommandHandler _handler;

    public ApplyForJobCommandHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _clientLookupMock = new Mock<IClientProfileLookupService>();
        _companyLookupMock = new Mock<ICompanyLookupService>();
        _handler = new ApplyForJobCommandHandler(
            _contextMock.Object,
            _clientLookupMock.Object,
            _companyLookupMock.Object);
    }

    private static ApplyForJobCommand ValidCommand() =>
        new(AdvertisementId: 10, CompanyId: 3, CandidateEmail: "marko.petrovic@email.rs");

    private void SetupApplicationsDbSet(List<JobApplicationEntity> applications)
    {
        var dbSetMock = applications.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSetMock.Object);
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private void SetupLookups()
    {
        _clientLookupMock
            .Setup(s => s.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientProfileLookupResult(5, "Marko", "Petrović", "marko.petrovic@email.rs"));

        _companyLookupMock
            .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanyLookupResult(3, "firma@kompanija.rs"));
    }

    [Fact]
    public async Task Kreira_prijavu_za_validnog_kandidata()
    {
        SetupApplicationsDbSet([]);
        SetupLookups();
        JobApplicationEntity? saved = null;
        _contextMock.Setup(c => c.JobApplications.Add(It.IsAny<JobApplicationEntity>()))
            .Callback<JobApplicationEntity>(a =>
            {
                JobApplicationTestHelper.SetId(a, 1);
                saved = a;
            });

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.AdvertisementId.Should().Be(10);
        result.CandidateId.Should().Be(5);
        result.CandidateEmail.Should().Be("marko.petrovic@email.rs");
        result.CompanyId.Should().Be(3);
        result.Status.Should().Be(0);

        saved.Should().NotBeNull();
        saved!.Status.Should().Be(JobLess.JobApplication.Domain.Enums.JobApplicationStatus.Pending);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Baca_izuzetak_kada_kandidat_ne_postoji()
    {
        SetupApplicationsDbSet([]);
        _clientLookupMock
            .Setup(s => s.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientProfileLookupResult?)null);

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Profil kandidata nije pronađen.");

        _contextMock.Verify(c => c.JobApplications.Add(It.IsAny<JobApplicationEntity>()), Times.Never);
    }

    [Fact]
    public async Task Baca_izuzetak_kada_kompanija_ne_postoji()
    {
        SetupApplicationsDbSet([]);
        _clientLookupMock
            .Setup(s => s.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientProfileLookupResult(5, "Marko", "Petrović", "marko.petrovic@email.rs"));
        _companyLookupMock
            .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompanyLookupResult?)null);

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Kompanija nije pronađena.");

        _contextMock.Verify(c => c.JobApplications.Add(It.IsAny<JobApplicationEntity>()), Times.Never);
    }

    [Fact]
    public async Task Ne_dozvoljava_duplu_prijavu_na_istom_oglasu()
    {
        SetupApplicationsDbSet(
        [
            JobApplicationTestHelper.CreatePendingApplication(
                id: 1,
                advertisementId: 10,
                candidateId: 5)
        ]);
        SetupLookups();

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Već ste prijavljeni na ovaj oglas.");

        _contextMock.Verify(c => c.JobApplications.Add(It.IsAny<JobApplicationEntity>()), Times.Never);
    }
}
