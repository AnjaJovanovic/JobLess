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

public class JobAppliedConsumerTests
{
    private readonly Mock<INotificationDbContext> _contextMock;
    private readonly JobAppliedConsumer _consumer;

    public JobAppliedConsumerTests()
    {
        _contextMock = new Mock<INotificationDbContext>();
        _consumer = new JobAppliedConsumer(_contextMock.Object, Mock.Of<ILogger<JobAppliedConsumer>>());
    }

    private static Mock<ConsumeContext<JobAppliedMessage>> CreateContext(JobAppliedMessage message)
    {
        var contextMock = new Mock<ConsumeContext<JobAppliedMessage>>();
        contextMock.Setup(c => c.Message).Returns(message);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return contextMock;
    }

    private static JobAppliedMessage NewMessage() => new(
        ApplicationId: 15,
        AdvertisementId: 4,
        ClientId: 7,
        ClientEmail: "pera.peric@gmail.com",
        ClientFirstName: "Pera",
        ClientLastName: "Perić",
        CompanyId: 2,
        CompanyEmail: "hr@tehnobit.rs");

    [Fact]
    public async Task Kompanija_dobija_notifikaciju_kada_kandidat_aplicira()
    {
        var message = NewMessage();
        var contextMock = CreateContext(message);
        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved.Should().NotBeNull();
        saved!.RecipientUserId.Should().Be("hr@tehnobit.rs");
        saved.Type.Should().Be(NotificationType.NewApplication);
        saved.Title.Should().Be("Nova prijava na oglas");
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Poruka_sadrzi_ime_i_prezime_kandidata()
    {
        var message = NewMessage();
        var contextMock = CreateContext(message);
        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved!.Message.Should().Contain("Pera").And.Contain("Perić").And.Contain("pera.peric@gmail.com");
    }

    [Fact]
    public async Task Notifikacija_cuva_id_prijave_oglasa_i_kompanije_za_frontend()
    {
        var message = NewMessage();
        var contextMock = CreateContext(message);
        NotificationEntity? saved = null;
        _contextMock.Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(n => saved = n);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _consumer.Consume(contextMock.Object);

        saved!.ApplicationId.Should().Be(15);
        saved.AdvertisementId.Should().Be(4);
        saved.CandidateId.Should().Be(7);
        saved.CompanyId.Should().Be(2);
    }
}
