using JobLess.Client.Domain.Enums;

namespace JobLess.Client.Application.Models;

public record CompanyApplicationDto(
    int ApplicationId,
    int ClientId,
    string FirstName,
    string LastName,
    string Email,
    int AdvertisementId,
    DateTime AppliedAt,
    JobApplicationStatus Status);
