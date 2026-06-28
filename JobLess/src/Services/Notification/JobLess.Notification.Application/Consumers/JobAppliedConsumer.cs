using JobLess.Contracts.Events;
using JobLess.Notification.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Notification.Application.Consumers;

public class JobAppliedConsumer(
    INotificationDbContext context,
    ILogger<JobAppliedConsumer> logger) : IConsumer<JobAppliedMessage>
{
    public async Task Consume(ConsumeContext<JobAppliedMessage> consumeContext)
    {
        var msg = consumeContext.Message;

        logger.LogInformation(
            "JobAppliedConsumer: candidate {ClientEmail} applied to ad {AdvertisementId}, notifying {CompanyEmail}",
            msg.ClientEmail, msg.AdvertisementId, msg.CompanyEmail);

        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            RecipientUserId = msg.CompanyEmail,
            Type = Domain.Enums.NotificationType.NewApplication,
            Title = "Nova prijava na oglas",
            Message = $"Kandidat {msg.ClientFirstName} {msg.ClientLastName} ({msg.ClientEmail}) se prijavio na oglas #{msg.AdvertisementId}.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync(consumeContext.CancellationToken);

        logger.LogInformation("Notification created for company {CompanyEmail}", msg.CompanyEmail);
    }
}
