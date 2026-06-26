using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Queries.GetApplicationsByAdvertisements;

public class GetApplicationsByAdvertisementsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetApplicationsByAdvertisementsQuery, IReadOnlyList<CompanyApplicationDto>>
{
    public async Task<IReadOnlyList<CompanyApplicationDto>> Handle(
        GetApplicationsByAdvertisementsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.AdvertisementIds.Count == 0)
            return Array.Empty<CompanyApplicationDto>();

        var query = context.JobApplications
            .AsNoTracking()
            .Include(a => a.Client)
            .Where(a => request.AdvertisementIds.Contains(a.AdvertisementId));

        if (request.Status.HasValue)
            query = query.Where(a => a.Status == request.Status.Value);

        return await query
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new CompanyApplicationDto(
                a.ApplicationId,
                a.ClientId,
                a.Client.FirstName,
                a.Client.LastName,
                a.Client.Email,
                a.AdvertisementId,
                a.AppliedAt,
                a.Status))
            .ToListAsync(cancellationToken);
    }
}
