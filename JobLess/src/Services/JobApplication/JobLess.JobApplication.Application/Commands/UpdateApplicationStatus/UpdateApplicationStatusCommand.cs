using JobLess.JobApplication.Application.Models;
using JobLess.JobApplication.Domain.Enums;
using MediatR;

namespace JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;

public record UpdateApplicationStatusCommand(
    int ApplicationId,
    string CompanyEmail,
    JobApplicationStatus Status
) : IRequest<JobApplicationDto>;
