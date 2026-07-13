using FluentAssertions;
using JobLess.Contracts.Events;
using JobLess.JobApplication.Application.Commands.ApplyForJob;
using JobLess.JobApplication.Application.Interfaces;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MassTransit;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class ApplyForJobCommandHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly Mock<IClientProfileLookupService> _clientLookupMock;
    private readonly Mock<ICompanyLookupService> _companyLookupMock;
    private readonly Mock<IAdvertisementLookupService> _adLookupMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly ApplyForJobCommandHandler _handler;

    public ApplyForJobCommandHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _clientLookupMock = new Mock<IClientProfileLookupService>();
        _companyLookupMock = new Mock<ICompanyLookupService>();
        _adLookupMock = new Mock<IAdvertisementLookupService>();
        _publishMock = new Mock<IPublishEndpoint>();

        _handler = new ApplyForJobCommandHandler(
            _contextMock.Object,
            _clientLookupMock.Object,
            _companyLookupMock.Object,
            _adLookupMock.Object,
            _publishMock.Object);
    }

    private static ApplyForJobCommand ValidCommand() =>
        new(AdvertisementId: 10, CompanyId: 2, CandidateEmail: "marko.petrovic@email.rs");

    private void SetupHappyPath(List<JobApplicationEntity>? existing = null)
    {
        _clientLookupMock
            .Setup(s => s.GetByEmailAsync("marko.petrovic@email.rs", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientProfileLookupResult(5, "Marko", "Petrović", "marko.petrovic@email.rs"));

        _adLookupMock
            .Setup(s => s.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdvertisementLookupResult(10, "Junior .NET Developer", 2));

        _companyLookupMock
            .Setup(s => s.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanyLookupResult(2, "hr@tehnobit.rs"));

        var dbSet = (existing ?? []).AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSet.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task Kandidat_moze_da_se_prijavi_na_oglas()
    {
        SetupHappyPath();
        JobApplicationEntity? saved = null;
        _contextMock.Setup(c => c.JobApplications.Add(It.IsAny<JobApplicationEntity>()))
            .Callback<JobApplicationEntity>(a => saved = a);

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.AdvertisementId.Should().Be(10);
        result.CandidateEmail.Should().Be("marko.petrovic@email.rs");
        result.CompanyId.Should().Be(2);
        result.Status.Should().Be(0);

        saved.Should().NotBeNull();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _publishMock.Verify(
            p => p.Publish(It.IsAny<JobAppliedMessage>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Baca_gresku_ako_profil_kandidata_ne_postoji()
    {
        _clientLookupMock
            .Setup(s => s.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientProfileLookupResult?)null);

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Profil kandidata nije pronađen.");
    }

    [Fact]
    public async Task Baca_gresku_ako_oglas_ne_postoji()
    {
        _clientLookupMock
            .Setup(s => s.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientProfileLookupResult(5, "Marko", "Petrović", "marko.petrovic@email.rs"));
        _adLookupMock
            .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdvertisementLookupResult?)null);

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Oglas nije pronađen.");
    }

    [Fact]
    public async Task Baca_gresku_ako_oglas_ne_pripada_kompaniji()
    {
        _clientLookupMock
            .Setup(s => s.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientProfileLookupResult(5, "Marko", "Petrović", "marko.petrovic@email.rs"));
        _adLookupMock
            .Setup(s => s.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdvertisementLookupResult(10, "Junior .NET Developer", CompanyId: 99));

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Oglas ne pripada navedenoj kompaniji.");
    }

    [Fact]
    public async Task Ne_dozvoljava_duplu_prijavu_na_isti_oglas()
    {
        var existing = JobApplicationTestHelper.CreateApplication();
        SetupHappyPath([existing]);

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Već ste prijavljeni na ovaj oglas.");

        _contextMock.Verify(c => c.JobApplications.Add(It.IsAny<JobApplicationEntity>()), Times.Never);
    }
}
