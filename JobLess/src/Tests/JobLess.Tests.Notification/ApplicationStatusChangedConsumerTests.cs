using FluentAssertions;
using JobLess.Contracts.Events;
using JobLess.Notification.Application.Consumers;
using JobLess.Notification.Application.Interfaces;
using JobLess.Notification.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Tests.Notification;

public class ApplicationStatusChangedConsumerTests
{
    private readonly Mock<INotificationDbContext> _contextMock;
    private readonly ApplicationStatusChangedConsumer _consumer;

    public ApplicationStatusChangedConsumerTests()
    {
        _contextMock = new Mock<INotificationDbContext>();
        _consumer = new ApplicationStatusChangedConsumer(_contextMock.Object, Mock.Of<ILogger<ApplicationStatusChangedConsumer>>());
    }

    private static Mock<ConsumeContext<ApplicationStatusChangedMessage>> CreateContext(ApplicationStatusChangedMessage message)
    {
        var contextMock = new Mock<ConsumeContext<ApplicationStatusChangedMessage>>();
        contextMock.Setup(c => c.Message).Returns(message);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return contextMock;
    }

    private static ApplicationStatusChangedMessage NewMessage(string newStatus) => new(
        ApplicationId: 22,
        AdvertisementId: 9,
        CompanyId: 3,
        CandidateEmail: "nikola.nikolic@gmail.com",
        CandidateFirstName: "Nikola",
        CandidateLastName: "Nikolić",
        NewStatus: newStatus);

    [Fact]
    public async Task Kandidat_dobija_notifikaciju_o_prihvatanju_kada_je_status_Accepted()
    {
        var message = NewMessage("Accepted");
        var contextMock = CreateContext(message);
        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved.Should().NotBeNull();
        saved!.Type.Should().Be(NotificationType.ApplicationAccepted);
        saved.Title.Should().Be("Prijava prihvaćena");
        saved.RecipientUserId.Should().Be("nikola.nikolic@gmail.com");
    }

    [Fact]
    public async Task Kandidat_dobija_notifikaciju_o_odbijanju_kada_je_status_Rejected()
    {
        var message = NewMessage("Rejected");
        var contextMock = CreateContext(message);
        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved.Should().NotBeNull();
        saved!.Type.Should().Be(NotificationType.ApplicationRejected);
        saved.Title.Should().Be("Prijava odbijena");
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("InReview")]
    public async Task Svaki_status_koji_nije_Accepted_tretira_se_kao_odbijanje(string status)
    {
        var message = NewMessage(status);
        var contextMock = CreateContext(message);
        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved!.Type.Should().Be(NotificationType.ApplicationRejected);
    }

    [Fact]
    public async Task Notifikacija_cuva_id_prijave_oglasa_i_kompanije()
    {
        var message = NewMessage("Accepted");
        var contextMock = CreateContext(message);
        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved!.ApplicationId.Should().Be(22);
        saved.AdvertisementId.Should().Be(9);
        saved.CompanyId.Should().Be(3);
    }
}
