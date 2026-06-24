using JobLess.Client.Domain.Enums;

namespace JobLess.Client.Domain.Entities;

public class JobApplication
{
    public int ApplicationId { get; set; }
    public int ClientId { get; set; }
    public int AdvertisementId { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;

    public Client Client { get; set; } = null!;
}
