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
using Jobless.Advertisement.Application.Queries.GetAllForCompany;

namespace JobLess.Advertisement.Application.Queries.GetAllForCompany;

public class GetAllForComapnyQueryHandler : IRequestHandler<GetAllForCompanyQuery, GetAllForCompanyResult>
{
    private readonly IApplicationDbContext _context;

    public GetAllForComapnyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllForCompanyResult> Handle(GetAllForCompanyQuery query, CancellationToken cancellationToken)
    {
        var advertisementsQuery = _context.JobAdvertisements
            .AsQueryable();
        var filteredQuery = advertisementsQuery
              .Where(x => x.CompanyId == query.CompanyId);

        var totalCount = await filteredQuery.CountAsync(cancellationToken);

        var advertisements = await filteredQuery
            .Select(AdvertisementModel.Projection)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new GetAllForCompanyResult
        {
            Advertisements = advertisements,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}