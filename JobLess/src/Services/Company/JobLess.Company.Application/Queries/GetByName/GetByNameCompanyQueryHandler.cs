using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace JobLess.Company.Application.Queries.GetByName
{
    public class GetByNameCompanyQueryHandler : IRequestHandler<GetByNameCompanyQuery, GetByNameCompanyResult>
    {
        private readonly IApplicationDbContext _context;

        public GetByNameCompanyQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetByNameCompanyResult> Handle(GetByNameCompanyQuery query, CancellationToken cancellationToken)
        {
            var companies = await _context.Companies
                .Where(x => x.Name.ToLower().Contains(query.Name.ToLower()) && x.IsActive)
                .Select(CompanyModel.Projection)
                .ToListAsync(cancellationToken);

            return new GetByNameCompanyResult
            {
                Companies = companies
            };
        }

    }
}
