using JobLess.Client.Domain.Enums;

namespace JobLess.Client.Application.Models;

public record JobApplicationDto(
    int ApplicationId,
    int ClientId,
    int AdvertisementId,
    DateTime AppliedAt,
    JobApplicationStatus Status);
