using JobLess.Notification.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Notification.Application.Commands.GetUserNotifications;

public class GetUserNotificationsQueryHandler(INotificationDbContext context)
    : IRequestHandler<GetUserNotificationsCommand, List<NotificationEntity>>
{
    public async Task<List<NotificationEntity>> Handle(
        GetUserNotificationsCommand request,
        CancellationToken cancellationToken)
    {
        return await context.Notifications
            .Where(n => n.RecipientUserId == request.UserEmailId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
