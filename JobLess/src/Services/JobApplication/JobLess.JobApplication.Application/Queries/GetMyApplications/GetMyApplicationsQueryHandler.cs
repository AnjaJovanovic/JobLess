using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Application.Queries.GetMyApplications;

public class GetMyApplicationsQueryHandler(IJobApplicationDbContext context)
    : IRequestHandler<GetMyApplicationsQuery, IReadOnlyList<JobApplicationDto>>
{
    public async Task<IReadOnlyList<JobApplicationDto>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        var email = request.CandidateEmail.Trim().ToLowerInvariant();

        var applications = await context.JobApplications
            .AsNoTracking()
            .Where(x => x.CandidateEmail.ToLower() == email)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new JobApplicationDto(
                x.Id,
                x.AdvertisementId,
                x.CandidateId,
                x.CandidateEmail,
                x.CandidateFirstName,
                x.CandidateLastName,
                x.CompanyId,
                x.CompanyEmail,
                (int)x.Status,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

        return applications;
    }
}
