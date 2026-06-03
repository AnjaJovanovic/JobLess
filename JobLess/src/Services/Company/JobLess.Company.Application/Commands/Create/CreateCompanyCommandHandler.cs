using JobLess.Company.Application.Interfaces;
using JobLess.Company.Domain.Company;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

            var exists = await _context.Companies.AnyAsync(x =>
                x.Email == request.Email ||
                x.TaxIdentificationNumber == request.TaxIdentificationNumber ||
                x.RegistrationNumber == request.RegistrationNumber,
                cancellationToken);

                        if (exists)
                        {
                            throw new Exception("Kompanija sa unetim PIB-om, matičnim brojem ili email adresom već postoji.");
                        }

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
