using JobLess.JobApplication.Domain.Enums;

namespace JobLess.JobApplication.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; }
    public Guid AdvertisementId { get; set; }
    public string CandidateUserId { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public string CompanyEmail { get; set; } = string.Empty;
    public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
