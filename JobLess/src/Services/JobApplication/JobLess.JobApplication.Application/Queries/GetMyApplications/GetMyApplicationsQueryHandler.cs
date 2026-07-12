using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Mappings;
using JobLess.JobApplication.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.JobApplication.Application.Queries.GetMyApplications;

public class GetMyApplicationsQueryHandler(
    IJobApplicationDbContext context,
    IAdvertisementLookupService advertisementLookupService)
    : IRequestHandler<GetMyApplicationsQuery, IReadOnlyList<JobApplicationDto>>
{
    public async Task<IReadOnlyList<JobApplicationDto>> Handle(
        GetMyApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var email = request.CandidateEmail.Trim().ToLowerInvariant();

        var applications = await context.JobApplications
            .AsNoTracking()
            .Where(x => x.CandidateEmail.ToLower() == email)
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
