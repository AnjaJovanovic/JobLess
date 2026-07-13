using FluentAssertions;
using JobLess.Notification.Application.Commands.GetUserNotifications;
using JobLess.Notification.Application.Interfaces;
using JobLess.Notification.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Tests.Notification;

public class GetUserNotificationsQueryHandlerTests
{
    private readonly Mock<INotificationDbContext> _contextMock;
    private readonly GetUserNotificationsQueryHandler _handler;

    public GetUserNotificationsQueryHandlerTests()
    {
        _contextMock = new Mock<INotificationDbContext>();
        _handler = new GetUserNotificationsQueryHandler(_contextMock.Object);
    }

    private void SetupNotifications(List<NotificationEntity> notifications)
    {
        var dbSetMock = notifications.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Notifications).Returns(dbSetMock.Object);
    }

    [Fact]
    public async Task Vraca_samo_notifikacije_prijavljenog_korisnika()
    {
        SetupNotifications(
        [
            new NotificationEntity
            {
                RecipientUserId = "jovana.jovanovic@gmail.com",
                Title = "Dobrodošli u JobLess!",
                Type = NotificationType.Welcome,
                CreatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc)
            },
            new NotificationEntity
            {
                RecipientUserId = "marko.markovic@drugafirma.rs",
                Title = "Nova prijava na oglas",
                Type = NotificationType.NewApplication,
                CreatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc)
            }
        ]);

        var result = await _handler.Handle(
            new GetUserNotificationsCommand("jovana.jovanovic@gmail.com"),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].RecipientUserId.Should().Be("jovana.jovanovic@gmail.com");
    }

    [Fact]
    public async Task Notifikacije_su_sortirane_od_najnovije_ka_najstarijoj()
    {
        SetupNotifications(
        [
            new NotificationEntity
            {
                RecipientUserId = "jovana.jovanovic@gmail.com",
                Title = "Prijava odbijena",
                Type = NotificationType.ApplicationRejected,
                CreatedAt = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new NotificationEntity
            {
                RecipientUserId = "jovana.jovanovic@gmail.com",
                Title = "Dobrodošli u JobLess!",
                Type = NotificationType.Welcome,
                CreatedAt = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            new NotificationEntity
            {
                RecipientUserId = "jovana.jovanovic@gmail.com",
                Title = "Prijava prihvaćena",
                Type = NotificationType.ApplicationAccepted,
                CreatedAt = new DateTime(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc)
            }
        ]);

        var result = await _handler.Handle(
            new GetUserNotificationsCommand("jovana.jovanovic@gmail.com"),
            CancellationToken.None);

        result.Should().HaveCount(3);
        result[0].Title.Should().Be("Prijava prihvaćena");
        result[1].Title.Should().Be("Prijava odbijena");
        result[2].Title.Should().Be("Dobrodošli u JobLess!");
    }

    [Fact]
    public async Task Vraca_praznu_listu_kada_korisnik_nema_notifikacija()
    {
        SetupNotifications([]);

        var result = await _handler.Handle(
            new GetUserNotificationsCommand("nemanja.nemanjic@gmail.com"),
            CancellationToken.None);

        result.Should().BeEmpty();
    }
}
