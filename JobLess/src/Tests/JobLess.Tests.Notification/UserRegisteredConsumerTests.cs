using FluentAssertions;
using JobLess.Contracts.Events;
using JobLess.Notification.Application.Consumers;
using JobLess.Notification.Application.Interfaces;
using JobLess.Notification.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Tests.Notification;

public class UserRegisteredConsumerTests
{
    private readonly Mock<INotificationDbContext> _contextMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly UserRegisteredConsumer _consumer;

    public UserRegisteredConsumerTests()
    {
        _contextMock = new Mock<INotificationDbContext>();
        _emailServiceMock = new Mock<IEmailService>();
        _consumer = new UserRegisteredConsumer(
            _contextMock.Object,
            _emailServiceMock.Object,
            Mock.Of<ILogger<UserRegisteredConsumer>>());

        var dbSetMock = new List<NotificationEntity>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Notifications).Returns(dbSetMock.Object);
    }

    private static Mock<ConsumeContext<UserRegisteredMessage>> CreateContext(UserRegisteredMessage message)
    {
        var contextMock = new Mock<ConsumeContext<UserRegisteredMessage>>();
        contextMock.Setup(c => c.Message).Returns(message);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return contextMock;
    }

    [Fact]
    public async Task Kreira_notifikaciju_dobrodoslice_za_novog_kandidata()
    {
        var message = new UserRegisteredMessage(Guid.NewGuid(), "pera.peric@gmail.com", "Client");
        var contextMock = CreateContext(message);

        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved.Should().NotBeNull();
        saved!.RecipientUserId.Should().Be("pera.peric@gmail.com");
        saved.Type.Should().Be(NotificationType.Welcome);
        saved.IsRead.Should().BeFalse();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Kreira_notifikaciju_dobrodoslice_i_za_novu_kompaniju()
    {
        var message = new UserRegisteredMessage(Guid.NewGuid(), "kontakt@tehnobit.rs", "Company");
        var contextMock = CreateContext(message);

        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved.Should().NotBeNull();
        saved!.RecipientUserId.Should().Be("kontakt@tehnobit.rs");
        saved.Type.Should().Be(NotificationType.Welcome);
    }

    [Fact]
    public async Task Salje_email_dobrodoslice_sa_ispravnom_ulogom()
    {
        var message = new UserRegisteredMessage(Guid.NewGuid(), "nikola.nikolic@yahoo.com", "Client");
        var contextMock = CreateContext(message);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        _emailServiceMock.Verify(
            e => e.SendWelcomeEmailAsync("nikola.nikolic@yahoo.com", "Client", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Notifikacija_ostaje_sacuvana_i_kada_slanje_emaila_ne_uspe()
    {
        // ako mejl server ne radi ne zelimo da cela obrada padne, notifikacija u app-u mora da ostane
        var message = new UserRegisteredMessage(Guid.NewGuid(), "jovana.jovanovic@outlook.com", "Client");
        var contextMock = CreateContext(message);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _emailServiceMock
            .Setup(e => e.SendWelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("SMTP server nedostupan"));

        var act = () => _consumer.Consume(contextMock.Object);

        await act.Should().NotThrowAsync();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
