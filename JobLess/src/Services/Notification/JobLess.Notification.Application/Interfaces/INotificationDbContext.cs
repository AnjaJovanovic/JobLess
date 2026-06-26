using Microsoft.EntityFrameworkCore;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Notification.Application.Interfaces;

public interface INotificationDbContext
{
    DbSet<NotificationEntity> Notifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
