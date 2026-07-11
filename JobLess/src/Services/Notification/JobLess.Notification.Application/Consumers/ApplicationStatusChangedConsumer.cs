using JobLess.Contracts.Events;
using JobLess.Notification.Application.Interfaces;
using JobLess.Notification.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Notification.Application.Consumers;

public class ApplicationStatusChangedConsumer(
    INotificationDbContext context,
    ILogger<ApplicationStatusChangedConsumer> logger) : IConsumer<ApplicationStatusChangedMessage>
{
    public async Task Consume(ConsumeContext<ApplicationStatusChangedMessage> consumeContext)
    {
        var msg = consumeContext.Message;
        var isAccepted = msg.NewStatus == "Accepted";

        logger.LogInformation(
            "ApplicationStatusChangedConsumer: application {ApplicationId} for {CandidateEmail} is {Status}",
            msg.ApplicationId, msg.CandidateEmail, msg.NewStatus);

        var type = isAccepted ? NotificationType.ApplicationAccepted : NotificationType.ApplicationRejected;
        var title = isAccepted ? "Prijava prihvaćena" : "Prijava odbijena";
        var message = isAccepted
            ? $"Čestitamo! Vaša prijava na oglas je prihvaćena."
            : $"Vaša prijava na oglas je odbijena.";

        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            RecipientUserId = msg.CandidateEmail,
            Type = type,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            ApplicationId = msg.ApplicationId,
            AdvertisementId = msg.AdvertisementId,
            CompanyId = msg.CompanyId
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync(consumeContext.CancellationToken);

        logger.LogInformation(
            "Notification sent to candidate {CandidateEmail}",
            msg.CandidateEmail);
    }
}
