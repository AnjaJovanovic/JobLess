namespace JobLess.JobApplication.Application.Models;

public record JobApplicationDto(
    int Id,
    int AdvertisementId,
    int CandidateId,
    string CandidateEmail,
    string CandidateFirstName,
    string CandidateLastName,
    int CompanyId,
    string CompanyEmail,
    int Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
