using JobLess.Notification.Domain.Enums;

namespace JobLess.Notification.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RecipientUserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public NotificationType Type { get; set; }
    
    // da bi forntend mogao da prikaze detaljnije notifikaciju
    public int? ApplicationId { get; set; }
    public int? AdvertisementId { get; set; }
    public int? CandidateId { get; set; }
    public int? CompanyId { get; set; }
}
