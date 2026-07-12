using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Mappings;
using JobLess.JobApplication.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Application.Queries.GetCompanyApplications;

public class GetCompanyApplicationsQueryHandler(
    IJobApplicationDbContext context,
    IAdvertisementLookupService advertisementLookupService)
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

        var applications = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var missingTitleIds = applications
            .Where(x => string.IsNullOrWhiteSpace(x.AdvertisementTitle))
            .Select(x => x.AdvertisementId);

        var titles = await advertisementLookupService.GetTitlesByIdsAsync(missingTitleIds, cancellationToken);

        return applications
            .Select(x => JobApplicationMapper.ToDto(
                x,
                string.IsNullOrWhiteSpace(x.AdvertisementTitle)
                    ? titles.GetValueOrDefault(x.AdvertisementId)
                    : null))
            .ToList();
    }
}
