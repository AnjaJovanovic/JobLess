using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Advertisement.Application.Queries.Search;

public class SearchAdvertisementQueryHandler : IRequestHandler<SearchAdvertisementQuery, SearchAdvertisementResult>
{
    private readonly IApplicationDbContext _context;

    public SearchAdvertisementQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SearchAdvertisementResult> Handle(
        SearchAdvertisementQuery query,
        CancellationToken cancellationToken)
    {
        var advertisementsQuery = _context.JobAdvertisements
            .Where(x => x.IsActive == true)
            .AsQueryable();

        // === FILTERS ===
        if (query.CompanyId.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.CompanyId == query.CompanyId.Value);

        if (!string.IsNullOrWhiteSpace(query.Title))
            advertisementsQuery = advertisementsQuery.Where(x => x.Title.Contains(query.Title));

        if (!string.IsNullOrWhiteSpace(query.Position))
            advertisementsQuery = advertisementsQuery.Where(x => x.Position.Contains(query.Position));

        if (query.ExpiresFrom.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.ExpiresAt >= query.ExpiresFrom.Value);

        if (query.ExpiresTo.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.ExpiresAt <= query.ExpiresTo.Value);

        if (query.EmploymentType.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.EmploymentType == query.EmploymentType.Value);

        if (query.WorkSchedule.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.WorkSchedule == query.WorkSchedule.Value);

        if (query.SeniorityLevel.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.SeniorityLevel == query.SeniorityLevel.Value);

        if (query.MinExperience.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.MinExperience >= query.MinExperience.Value);

        if (query.MaxExperience.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.MaxExperience <= query.MaxExperience.Value);

        if (!string.IsNullOrWhiteSpace(query.City))
            advertisementsQuery = advertisementsQuery.Where(x => x.City.Contains(query.City));

        if (!string.IsNullOrWhiteSpace(query.Country))
            advertisementsQuery = advertisementsQuery.Where(x => x.Country!.Contains(query.Country));

        if (query.WorkType.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.WorkType == query.WorkType.Value);

        if (query.SalaryFrom.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.SalaryFrom >= query.SalaryFrom.Value);

        if (query.SalaryTo.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.SalaryTo <= query.SalaryTo.Value);

        if (!string.IsNullOrWhiteSpace(query.Currency))
            advertisementsQuery = advertisementsQuery.Where(x => x.Currency == query.Currency);

        if (query.IsSalaryVisible.HasValue)
            advertisementsQuery = advertisementsQuery.Where(x => x.IsSalaryVisible == query.IsSalaryVisible.Value);

        var totalCount = await advertisementsQuery.CountAsync(cancellationToken);

        var advertisements = await advertisementsQuery
            .Select(AdvertisementModel.Projection)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new SearchAdvertisementResult
        {
            Advertisements = advertisements,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}