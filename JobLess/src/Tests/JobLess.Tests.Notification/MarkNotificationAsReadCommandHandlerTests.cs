using FluentAssertions;
using JobLess.Notification.Application.Commands.MarkNotificationAsRead;
using JobLess.Notification.Application.Interfaces;
using JobLess.Notification.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Tests.Notification;

public class MarkNotificationAsReadCommandHandlerTests
{
    private readonly Mock<INotificationDbContext> _contextMock;
    private readonly MarkNotificationAsReadCommandHandler _handler;

    public MarkNotificationAsReadCommandHandlerTests()
    {
        _contextMock = new Mock<INotificationDbContext>();
        _handler = new MarkNotificationAsReadCommandHandler(_contextMock.Object);
    }

    private void SetupNotifications(List<NotificationEntity> notifications)
    {
        var dbSetMock = notifications.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Notifications).Returns(dbSetMock.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task Oznacava_notifikaciju_kao_procitanu_i_vraca_true()
    {
        var notificationId = Guid.NewGuid();
        var notification = new NotificationEntity
        {
            Id = notificationId,
            RecipientUserId = "pera.peric@gmail.com",
            Title = "Prijava prihvaćena",
            Type = NotificationType.ApplicationAccepted,
            IsRead = false
        };
        SetupNotifications([notification]);

        var result = await _handler.Handle(
            new MarkNotificationAsReadCommand(notificationId, "pera.peric@gmail.com"),
            CancellationToken.None);

        result.Should().BeTrue();
        notification.IsRead.Should().BeTrue();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Vraca_false_kada_notifikacija_ne_postoji()
    {
        SetupNotifications([]);

        var result = await _handler.Handle(
            new MarkNotificationAsReadCommand(Guid.NewGuid(), "pera.peric@gmail.com"),
            CancellationToken.None);

        result.Should().BeFalse();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Ne_dozvoljava_da_tudja_notifikacija_bude_oznacena_kao_procitana()
    {
        var notificationId = Guid.NewGuid();
        var notification = new NotificationEntity
        {
            Id = notificationId,
            RecipientUserId = "nikola.nikolic@gmail.com",
            Title = "Nova prijava na oglas",
            Type = NotificationType.NewApplication,
            IsRead = false
        };
        SetupNotifications([notification]);

        var result = await _handler.Handle(
            new MarkNotificationAsReadCommand(notificationId, "neko.drugi@gmail.com"),
            CancellationToken.None);

        result.Should().BeFalse();
        notification.IsRead.Should().BeFalse();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
