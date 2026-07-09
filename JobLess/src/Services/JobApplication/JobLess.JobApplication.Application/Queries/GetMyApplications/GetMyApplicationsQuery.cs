using JobLess.JobApplication.Application.Models;
using MediatR;

namespace JobLess.JobApplication.Application.Queries.GetMyApplications;

public record GetMyApplicationsQuery(string CandidateEmail) : IRequest<IReadOnlyList<JobApplicationDto>>;
