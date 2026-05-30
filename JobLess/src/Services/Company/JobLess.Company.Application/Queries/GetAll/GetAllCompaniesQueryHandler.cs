using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Queries.GetAll
{
    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, GetAllCompaniesResult>
    {
        private readonly IApplicationDbContext _context;

        public GetAllCompaniesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetAllCompaniesResult> Handle(GetAllCompaniesQuery query, CancellationToken cancellationToken)
        {
            var companiesQuery = _context.Companies
                .Where(x => x.IsActive)
                .AsQueryable();

            var totalCount = await companiesQuery.CountAsync(cancellationToken);

            var companies = await companiesQuery
                .Select(CompanyModel.Projection)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            return new GetAllCompaniesResult
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
