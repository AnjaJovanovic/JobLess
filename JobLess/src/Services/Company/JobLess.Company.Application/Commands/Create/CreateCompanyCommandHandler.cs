using JobLess.Company.Application.Common.Helpers;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Domain.Entities;
using JobLess.Company.Domain.Enums;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Company.Application.Commands.Create
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;

        public CreateCompanyCommandHandler(IApplicationDbContext context, IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = new Domain.Entities.Company
            {
                Name = request.Name,
                Description = request.Description,
                //Industry = request.Industry,
                Industry = IndustryHelper.GetIndustry(request.Industry),
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
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                CompanySize = request.CompanySize,
               // CompanySize = GetCompanySize(request.EmployeeCount),
                //EmployeeCount = request.EmployeeCount,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var companyExists = await _context.Companies
                .Where(x => (x.Email == request.Email || x.TaxIdentificationNumber == request.TaxIdentificationNumber || x.RegistrationNumber == request.RegistrationNumber) && x.IsActive == true)
                .FirstOrDefaultAsync(cancellationToken);

            if (companyExists != null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id",
                        "Kompanija sa unetim PIB-om, matičnim brojem ili email adresom već postoji.");
                throw new InvalidOperationException();
            }/*
            var exists = await _context.Companies.AnyAsync(x =>
                x.Email == request.Email ||
                x.TaxIdentificationNumber == request.TaxIdentificationNumber ||
                x.RegistrationNumber == request.RegistrationNumber,
                cancellationToken);

                        if (exists)
                        {
                            throw new Exception("Kompanija sa unetim PIB-om, matičnim brojem ili email adresom već postoji.");
                        }
            */

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

        private static CompanySize GetCompanySize(int employeeCount)
        {
            if (employeeCount <= 10)
                return CompanySize.OneToTen;

            if (employeeCount <= 50)
                return CompanySize.ElevenToFifty;

            if (employeeCount <= 200)
                return CompanySize.FiftyOneToTwoHundred;

            if (employeeCount <= 500)
                return CompanySize.TwoHundredOneToFiveHundred;

            return CompanySize.MoreThanFiveHundred;
        }
    }
}
