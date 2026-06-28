using MediatR;

namespace JobLess.Notification.Application.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(Guid NotificationId, string UserEmail) : IRequest<bool>;
