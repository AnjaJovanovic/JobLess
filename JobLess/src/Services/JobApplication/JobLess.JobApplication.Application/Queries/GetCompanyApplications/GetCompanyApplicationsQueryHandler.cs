using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Application.Queries.GetCompanyApplications;

public class GetCompanyApplicationsQueryHandler(IJobApplicationDbContext context)
    : IRequestHandler<GetCompanyApplicationsQuery, IReadOnlyList<JobApplicationDto>>
{
    public async Task<IReadOnlyList<JobApplicationDto>> Handle(
        GetCompanyApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var email = request.CompanyEmail.Trim().ToLowerInvariant();

        var query = context.JobApplications
            .AsNoTracking()
            .Where(x => x.CompanyEmail.ToLower() == email);

        if (request.AdvertisementId.HasValue)
        {
            query = query.Where(x => x.AdvertisementId == request.AdvertisementId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        return await query
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
    }
}
