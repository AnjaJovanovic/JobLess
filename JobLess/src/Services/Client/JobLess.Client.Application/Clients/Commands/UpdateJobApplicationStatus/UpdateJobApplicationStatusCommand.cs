using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Enums;
using MediatR;

namespace JobLess.Client.Application.Clients.Commands.UpdateJobApplicationStatus;

public record UpdateJobApplicationStatusCommand(
    int ApplicationId,
    JobApplicationStatus Status) : IRequest<JobApplicationDto>;
