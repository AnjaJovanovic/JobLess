using FluentAssertions;
using JobLess.Contracts.Events;
using JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Domain.Enums;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MassTransit;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class UpdateApplicationStatusCommandHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly UpdateApplicationStatusCommandHandler _handler;

    public UpdateApplicationStatusCommandHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _publishMock = new Mock<IPublishEndpoint>();
        _handler = new UpdateApplicationStatusCommandHandler(_contextMock.Object, _publishMock.Object);
    }

    private void SetupApplications(params JobApplicationEntity[] applications)
    {
        var dbSet = applications.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSet.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task Kompanija_moze_da_prihvati_prijavu()
    {
        var application = JobApplicationTestHelper.CreateApplication();
        SetupApplications(application);

        var result = await _handler.Handle(
            new UpdateApplicationStatusCommand(1, "hr@tehnobit.rs", JobApplicationStatus.Accepted),
            CancellationToken.None);

        result.Status.Should().Be((int)JobApplicationStatus.Accepted);
        application.Status.Should().Be(JobApplicationStatus.Accepted);
        _publishMock.Verify(
            p => p.Publish(It.IsAny<ApplicationStatusChangedMessage>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Kompanija_moze_da_odbije_prijavu()
    {
        var application = JobApplicationTestHelper.CreateApplication();
        SetupApplications(application);

        var result = await _handler.Handle(
            new UpdateApplicationStatusCommand(1, "hr@tehnobit.rs", JobApplicationStatus.Rejected),
            CancellationToken.None);

        result.Status.Should().Be((int)JobApplicationStatus.Rejected);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Baca_gresku_ako_prijava_ne_postoji()
    {
        SetupApplications();

        var act = () => _handler.Handle(
            new UpdateApplicationStatusCommand(99, "hr@tehnobit.rs", JobApplicationStatus.Accepted),
            CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Prijava nije pronađena.");
    }

    [Fact]
    public async Task Baca_gresku_ako_kompanija_nije_vlasnik_prijave()
    {
        var application = JobApplicationTestHelper.CreateApplication();
        SetupApplications(application);

        var act = () => _handler.Handle(
            new UpdateApplicationStatusCommand(1, "neko@druga.rs", JobApplicationStatus.Accepted),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Nemate dozvolu da menjate ovu prijavu.");
    }

    [Fact]
    public async Task Ne_dozvoljava_promenu_statusa_ako_je_prijava_vec_obradjena()
    {
        var application = JobApplicationTestHelper.CreateApplication();
        application.Accept();
        SetupApplications(application);

        var act = () => _handler.Handle(
            new UpdateApplicationStatusCommand(1, "hr@tehnobit.rs", JobApplicationStatus.Rejected),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
