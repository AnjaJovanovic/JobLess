using FluentAssertions;
using JobLess.Contracts.Events;
using JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Domain.Enums;
using JobLess.Notification.Application.Commands.GetUserNotifications;
using JobLess.Notification.Application.Consumers;
using JobLess.Notification.Application.Interfaces;
using JobLess.Notification.Domain.Enums;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MassTransit;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Tests.Notification;


public class ApplicationAcceptedNotificationIntegrationTests
{
    [Fact]
    public async Task Should_Create_Notification_When_Application_Is_Accepted()
    {
        var application = JobApplicationTestHelper.CreateApplication(
            id: 42,
            advertisementId: 9,
            candidateEmail: "marko.petrovic@email.rs",
            candidateFirstName: "Marko",
            candidateLastName: "Petrović",
            companyId: 3,
            companyEmail: "hr@tehnobit.rs");

        var jobApplicationContext = new Mock<IJobApplicationDbContext>();
        var applications = new List<JobApplicationEntity> { application }
            .AsQueryable()
            .BuildMockDbSet();

        jobApplicationContext
            .Setup(c => c.JobApplications)
            .Returns(applications.Object);

        jobApplicationContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        ApplicationStatusChangedMessage? publishedEvent = null;

        var publishEndpoint = new Mock<IPublishEndpoint>();

        publishEndpoint
            .Setup(p => p.Publish(
                It.IsAny<ApplicationStatusChangedMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<ApplicationStatusChangedMessage, CancellationToken>(
                (message, _) => publishedEvent = message)
            .Returns(Task.CompletedTask);

        var updateHandler = new UpdateApplicationStatusCommandHandler(
            jobApplicationContext.Object,
            publishEndpoint.Object);

        // Act - accept application
        var result = await updateHandler.Handle(
            new UpdateApplicationStatusCommand(
                42,
                "hr@tehnobit.rs",
                JobApplicationStatus.Accepted),
            CancellationToken.None);

        // Assert - event published
        result.Status.Should().Be((int)JobApplicationStatus.Accepted);

        publishedEvent.Should().NotBeNull();
        publishedEvent!.NewStatus.Should().Be("Accepted");
        publishedEvent.CandidateEmail.Should().Be("marko.petrovic@email.rs");

        // Arrange - notification consumer
        var notifications = new List<NotificationEntity>();

        var notificationContext = new Mock<INotificationDbContext>();

        notificationContext
            .Setup(c => c.Notifications.Add(It.IsAny<NotificationEntity>()))
            .Callback<NotificationEntity>(notification => notifications.Add(notification));

        notificationContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var consumer = new ApplicationStatusChangedConsumer(
            notificationContext.Object,
            Mock.Of<ILogger<ApplicationStatusChangedConsumer>>());

        var consumeContext = new Mock<ConsumeContext<ApplicationStatusChangedMessage>>();

        consumeContext
            .Setup(c => c.Message)
            .Returns(publishedEvent!);

        consumeContext
            .Setup(c => c.CancellationToken)
            .Returns(CancellationToken.None);

        // Act - consume event
        await consumer.Consume(consumeContext.Object);

        // Assert - notification created
        notifications.Should().HaveCount(1);

        notifications[0].Type.Should().Be(NotificationType.ApplicationAccepted);
        notifications[0].Title.Should().Be("Prijava prihvaćena");
        notifications[0].RecipientUserId.Should().Be("marko.petrovic@email.rs");

        // Arrange - query notifications
        var notificationDbSet = notifications
            .AsQueryable()
            .BuildMockDbSet();

        notificationContext
            .Setup(c => c.Notifications)
            .Returns(notificationDbSet.Object);

        var queryHandler = new GetUserNotificationsQueryHandler(
            notificationContext.Object);

        // Act
        var userNotifications = await queryHandler.Handle(
            new GetUserNotificationsCommand("marko.petrovic@email.rs"),
            CancellationToken.None);

        // Assert
        userNotifications.Should().HaveCount(1);

        userNotifications[0].Title.Should().Be("Prijava prihvaćena");
        userNotifications[0].Type.Should().Be(NotificationType.ApplicationAccepted);
        userNotifications[0].ApplicationId.Should().Be(42);
        userNotifications[0].IsRead.Should().BeFalse();
    }
}


file static class JobApplicationTestHelper
{
    public static JobApplicationEntity CreateApplication(
        int id,
        int advertisementId,
        string candidateEmail,
        string candidateFirstName,
        string candidateLastName,
        int companyId,
        string companyEmail)
    {
        var application = JobApplicationEntity.Create(
            advertisementId,
            "Junior .NET Developer",
            candidateId: 5,
            candidateEmail,
            candidateFirstName,
            candidateLastName,
            companyId,
            companyEmail);

        typeof(JobApplicationEntity)
            .GetProperty(nameof(JobApplicationEntity.Id))!
            .SetValue(application, id);

        return application;
    }
}