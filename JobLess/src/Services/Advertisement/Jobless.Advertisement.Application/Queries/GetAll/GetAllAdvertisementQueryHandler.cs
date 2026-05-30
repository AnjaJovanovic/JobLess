using JobLess.Advertisement.Application.Models;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobLess.Advertisement.Application.Queries.GetAll;
using System.Data;

namespace JobLess.Advertisement.Application.Queries.GetAll;

public class GetAllAdvertisementQueryHandler : IRequestHandler<GetAllAdvertisementQuery, GetAllAdvertisementResult>
{
    private readonly IApplicationDbContext _context;

    public GetAllAdvertisementQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllAdvertisementResult> Handle(GetAllAdvertisementQuery query, CancellationToken cancellationToken)
    {
        var advertisementsQuery = _context.JobAdvertisements
            .AsQueryable();

        var totalCount = await advertisementsQuery.CountAsync(cancellationToken);

        var advertisements = await advertisementsQuery
            .Where(x => x.IsActive == true && (x.ExpiresAt == null || x.ExpiresAt >= DateTime.UtcNow))
            .Select(AdvertisementModel.Projection)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new GetAllAdvertisementResult
        {
            Advertisements = advertisements,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}