using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Queries.GetClientApplications;

public class GetClientApplicationsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetClientApplicationsQuery, IReadOnlyList<JobApplicationDto>>
{
    public async Task<IReadOnlyList<JobApplicationDto>> Handle(
        GetClientApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.JobApplications
            .AsNoTracking()
            .Where(a => a.ClientId == request.ClientId)
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new JobApplicationDto(
                a.ApplicationId,
                a.ClientId,
                a.AdvertisementId,
                a.AppliedAt,
                a.Status))
            .ToListAsync(cancellationToken);
    }
}
