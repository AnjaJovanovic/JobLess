using JobLess.Company.Application.Interfaces;
using JobLess.Company.Domain.Company;
using JobLess.Company.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Commands.Create
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateCompanyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = new Domain.Entities.Company
            {
                Name = request.Name,
                Description = request.Description,
                Industry = request.Industry,
                Website = request.Website,
                Location = request.Location,
                TaxIdentificationNumber = request.TaxIdentificationNumber,
                RegistrationNumber = request.RegistrationNumber,
                OwnerName = request.OwnerName,
                ContactPersonFirstName = request.ContactPersonFirstName,
                ContactPersonLastName = request.ContactPersonLastName,
                ContactPersonPhoneNumber = request.ContactPersonPhoneNumber,
                ContactPersonPosition = request.ContactPersonPosition,
                Email = request.Email,
                PasswordHash = request.PasswordHash,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                CompanySize = request.CompanySize,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

        _context.Companies.Add(company);
            await _context.SaveChangesAsync(cancellationToken);

            var admin = new CompanyAdmin
            {
                UserId = request.OwnerId,
                Role = "Owner",
                CreatedAt = DateTime.UtcNow
            };

            _context.CompanyAdmins.Add(admin);
            await _context.SaveChangesAsync(cancellationToken);

            return company.Id;
        }
    }
}
