using JobLess.JobApplication.Domain.Enums;
using JobLess.JobApplication.Domain.Exceptions;

namespace JobLess.JobApplication.Domain.Entities;

public class JobApplication
{
    public int Id { get; private set; }
    public int AdvertisementId { get; private set; }
    public string AdvertisementTitle { get; private set; } = string.Empty;
    public int CandidateId { get; private set; }
    public string CandidateEmail { get; private set; } = string.Empty;
    public string CandidateFirstName { get; private set; } = string.Empty;
    public string CandidateLastName { get; private set; } = string.Empty;
    public int CompanyId { get; private set; }
    public string CompanyEmail { get; private set; } = string.Empty;
    public JobApplicationStatus Status { get; private set; } = JobApplicationStatus.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private JobApplication()
    {
    }

    public static JobApplication Create(
        int advertisementId,
        string advertisementTitle,
        int candidateId,
        string candidateEmail,
        string candidateFirstName,
        string candidateLastName,
        int companyId,
        string companyEmail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(advertisementTitle);
        ArgumentException.ThrowIfNullOrWhiteSpace(candidateEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(candidateFirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(candidateLastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(companyEmail);

        if (advertisementId <= 0)
            throw new ArgumentOutOfRangeException(nameof(advertisementId));
        if (candidateId <= 0)
            throw new ArgumentOutOfRangeException(nameof(candidateId));
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId));

        return new JobApplication
        {
            AdvertisementId = advertisementId,
            AdvertisementTitle = advertisementTitle.Trim(),
            CandidateId = candidateId,
            CandidateEmail = candidateEmail.Trim(),
            CandidateFirstName = candidateFirstName.Trim(),
            CandidateLastName = candidateLastName.Trim(),
            CompanyId = companyId,
            CompanyEmail = companyEmail.Trim(),
            Status = JobApplicationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Accept()
    {
        EnsurePendingStatus(JobApplicationStatus.Accepted);
        Status = JobApplicationStatus.Accepted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        EnsurePendingStatus(JobApplicationStatus.Rejected);
        Status = JobApplicationStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsurePendingStatus(JobApplicationStatus targetStatus)
    {
        if (Status != JobApplicationStatus.Pending)
            throw new InvalidJobApplicationStatusTransitionException(Status, targetStatus);
    }
}
