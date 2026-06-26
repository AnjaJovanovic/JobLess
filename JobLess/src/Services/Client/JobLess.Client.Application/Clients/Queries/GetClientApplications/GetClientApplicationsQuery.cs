using JobLess.Client.Application.Models;
using MediatR;

namespace JobLess.Client.Application.Clients.Queries.GetClientApplications;

public record GetClientApplicationsQuery(int ClientId) : IRequest<IReadOnlyList<JobApplicationDto>>;
