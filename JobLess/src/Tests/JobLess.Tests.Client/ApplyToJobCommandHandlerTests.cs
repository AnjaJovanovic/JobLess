using ClientEntity = JobLess.Client.Domain.Entities.Client;
using FluentAssertions;
using JobLess.Client.Application.Clients.Commands.ApplyToJob;
using JobLess.Client.Application.Interfaces;
using JobLess.Client.Domain.Entities;
using JobLess.Client.Domain.Enums;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.Client;

public class ApplyToJobCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly ApplyToJobCommandHandler _handler;

    public ApplyToJobCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new ApplyToJobCommandHandler(_contextMock.Object);
    }

    private static ClientEntity PostojeciKandidat() =>
        new()
        {
            ClientId = 1,
            Email = "marko.petrovic@email.rs",
            PasswordHash = string.Empty,
            FirstName = "Marko",
            LastName = "Petrović",
            Gender = Gender.Male,
            IsActive = true
        };

    private void SetupDatabase(List<ClientEntity> clients, List<JobApplication> applications)
    {
        _contextMock.Setup(c => c.Clients).Returns(clients.AsQueryable().BuildMockDbSet().Object);
        _contextMock.Setup(c => c.JobApplications).Returns(applications.AsQueryable().BuildMockDbSet().Object);
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Prijavljuje_kandidata_na_oglas()
    {
        SetupDatabase([PostojeciKandidat()], []);
        JobApplication? sacuvanaPrijava = null;

        _contextMock.Setup(c => c.JobApplications.Add(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a =>
            {
                a.ApplicationId = 10;
                sacuvanaPrijava = a;
            });

        var result = await _handler.Handle(new ApplyToJobCommand(1, 42), CancellationToken.None);

        result.ApplicationId.Should().Be(10);
        result.ClientId.Should().Be(1);
        result.AdvertisementId.Should().Be(42);
        result.Status.Should().Be(JobApplicationStatus.Pending);

        sacuvanaPrijava.Should().NotBeNull();
        sacuvanaPrijava!.Status.Should().Be(JobApplicationStatus.Pending);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Ne_dozvoljava_duplu_prijavu_na_ist_oglas()
    {
        SetupDatabase(
            [PostojeciKandidat()],
            [
                new JobApplication
                {
                    ApplicationId = 5,
                    ClientId = 1,
                    AdvertisementId = 42,
                    AppliedAt = DateTime.UtcNow,
                    Status = JobApplicationStatus.Pending
                }
            ]);

        var act = () => _handler.Handle(new ApplyToJobCommand(1, 42), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Već ste prijavljeni na ovaj oglas.");

        _contextMock.Verify(c => c.JobApplications.Add(It.IsAny<JobApplication>()), Times.Never);
    }
}
