using JobLess.Contracts.Events;
using JobLess.Notification.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Notification.Application.Consumers;

public class UserRegisteredConsumer(
    INotificationDbContext context,
    IEmailService emailService,
    ILogger<UserRegisteredConsumer> logger) : IConsumer<UserRegisteredMessage>
{
    public async Task Consume(ConsumeContext<UserRegisteredMessage> consumeContext)
    {
        var msg = consumeContext.Message;

        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            RecipientUserId = msg.UserId.ToString(),
            Type = Domain.Enums.NotificationType.Welcome,
            Title = "Dobrodošli u JobLess!",
            Message = "Vaš nalog je uspešno kreiran. Srećno pri zapošljavanju!",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync(consumeContext.CancellationToken);

        try
        {
            await emailService.SendWelcomeEmailAsync(msg.Email, msg.Role, consumeContext.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send welcome email to {Email}", msg.Email);
        }
    }
}
