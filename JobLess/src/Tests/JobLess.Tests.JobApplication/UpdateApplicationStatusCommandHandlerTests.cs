using FluentAssertions;
using JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Domain.Enums;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class UpdateApplicationStatusCommandHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly UpdateApplicationStatusCommandHandler _handler;

    public UpdateApplicationStatusCommandHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _handler = new UpdateApplicationStatusCommandHandler(_contextMock.Object);
    }

    private void SetupApplicationsDbSet(List<JobApplicationEntity> applications)
    {
        var dbSetMock = applications.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSetMock.Object);
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Prihvata_prijavu_kada_kompanija_ima_dozvolu()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication(
            id: 1,
            companyEmail: "firma@kompanija.rs");
        SetupApplicationsDbSet([application]);

        var command = new UpdateApplicationStatusCommand(
            ApplicationId: 1,
            CompanyEmail: "FIRMA@kompanija.rs",
            Status: JobApplicationStatus.Accepted);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be((int)JobApplicationStatus.Accepted);
        application.Status.Should().Be(JobApplicationStatus.Accepted);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Odbija_prijavu_kada_kompanija_ima_dozvolu()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication(
            id: 1,
            companyEmail: "firma@kompanija.rs");
        SetupApplicationsDbSet([application]);

        var command = new UpdateApplicationStatusCommand(
            ApplicationId: 1,
            CompanyEmail: "firma@kompanija.rs",
            Status: JobApplicationStatus.Rejected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be((int)JobApplicationStatus.Rejected);
        application.Status.Should().Be(JobApplicationStatus.Rejected);
    }

    [Fact]
    public async Task Baca_izuzetak_kada_prijava_ne_postoji()
    {
        SetupApplicationsDbSet([]);

        var command = new UpdateApplicationStatusCommand(
            ApplicationId: 99,
            CompanyEmail: "firma@kompanija.rs",
            Status: JobApplicationStatus.Accepted);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Prijava nije pronađena.");
    }

    [Fact]
    public async Task Baca_izuzetak_kada_kompanija_nema_dozvolu()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication(
            id: 1,
            companyEmail: "firma@kompanija.rs");
        SetupApplicationsDbSet([application]);

        var command = new UpdateApplicationStatusCommand(
            ApplicationId: 1,
            CompanyEmail: "druga@kompanija.rs",
            Status: JobApplicationStatus.Accepted);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Nemate dozvolu da menjate ovu prijavu.");

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Baca_izuzetak_kada_status_nije_pending()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication(
            id: 1,
            companyEmail: "firma@kompanija.rs");
        application.Accept();
        SetupApplicationsDbSet([application]);

        var command = new UpdateApplicationStatusCommand(
            ApplicationId: 1,
            CompanyEmail: "firma@kompanija.rs",
            Status: JobApplicationStatus.Rejected);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Accepted*Rejected*");

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
