using JobLess.JobApplication.Application.Models;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;

namespace JobLess.JobApplication.Application.Mappings;

public static class JobApplicationMapper
{
    public static JobApplicationDto ToDto(JobApplicationEntity application, string? advertisementTitle = null)
    {
        return new JobApplicationDto(
            application.Id,
            application.AdvertisementId,
            advertisementTitle ?? application.AdvertisementTitle,
            application.CandidateId,
            application.CandidateEmail,
            application.CandidateFirstName,
            application.CandidateLastName,
            application.CompanyId,
            application.CompanyEmail,
            (int)application.Status,
            application.CreatedAt,
            application.UpdatedAt);
    }
}
