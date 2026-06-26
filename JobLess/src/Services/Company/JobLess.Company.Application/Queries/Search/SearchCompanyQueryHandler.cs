using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Models;
using JobLess.Company.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Queries.Search
{
    public class SearchCompanyQueryHandler : IRequestHandler<SearchCompanyQuery, SearchCompanyResult>
    {
        private readonly IApplicationDbContext _context;

        public SearchCompanyQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SearchCompanyResult> Handle(SearchCompanyQuery query, CancellationToken cancellationToken)
        {
            var companiesQuery = _context.Companies
                .Where(x => x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                companiesQuery = companiesQuery.Where(x => x.Name.Contains(query.Name));

            if (query.Industry != null)
            {
                var industryEnum = Enum.Parse<Industry>(query.Industry);

                companiesQuery = companiesQuery
                    .Where(x => x.Industry == industryEnum);
            }

            if (!string.IsNullOrWhiteSpace(query.Location))
                companiesQuery = companiesQuery.Where(x => x.Location.Contains(query.Location));

            var totalCount = await companiesQuery.CountAsync(cancellationToken);

            var companies = await companiesQuery
                .Select(CompanyModel.Projection)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            return new SearchCompanyResult
            {
                Companies = companies,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
