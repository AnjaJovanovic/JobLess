using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Company.Application.Queries.GetOne
{
    public class GetOneCompanyQueryHandler : IRequestHandler<GetOneCompanyQuery, GetOneCompanyResult>
    {
        private readonly IApplicationDbContext _context;

        public GetOneCompanyQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetOneCompanyResult?> Handle(GetOneCompanyQuery query, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Where(x => x.Id == query.Id && x.IsActive)
                .Select(CompanyModel.Projection)
                .FirstOrDefaultAsync(cancellationToken);

            if (company == null)
                return null;

            return new GetOneCompanyResult
            {
                Company = company
            };
        }
    }
}
