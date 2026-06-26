using MediatR;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Notification.Application.Commands.GetUserNotifications;

public record GetUserNotificationsCommand(Guid UserId) : IRequest<List<NotificationEntity>>;
