using JobLess.Notification.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Notification.Application.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler(INotificationDbContext context)
    : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await context.Notifications
            .FirstOrDefaultAsync(
                n => n.Id == request.NotificationId && n.RecipientUserId == request.UserEmail,
                cancellationToken);

        if (notification is null)
            return false;

        notification.IsRead = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
