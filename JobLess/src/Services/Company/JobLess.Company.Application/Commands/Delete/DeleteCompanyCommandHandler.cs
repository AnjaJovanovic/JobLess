using JobLess.Company.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Company.Application.Commands.Delete
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCompanyCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.IsActive, cancellationToken);

            if (company == null)
                return false;

            company.IsActive = false;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
