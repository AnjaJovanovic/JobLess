using JobLess.JobApplication.Application.Models;
using MediatR;

namespace JobLess.JobApplication.Application.Commands.ApplyForJob;

public record ApplyForJobCommand(
    int AdvertisementId,
    int CompanyId,
    string CandidateEmail
) : IRequest<JobApplicationDto>;
