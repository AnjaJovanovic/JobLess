using JobLess.JobApplication.Application.Models;
using JobLess.JobApplication.Domain.Enums;
using MediatR;

namespace JobLess.JobApplication.Application.Queries.GetCompanyApplications;

public record GetCompanyApplicationsQuery(
    string CompanyEmail,
    int? AdvertisementId = null,
    JobApplicationStatus? Status = null
) : IRequest<IReadOnlyList<JobApplicationDto>>;
