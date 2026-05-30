using JobLess.Company.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace JobLess.Company.Application.Commands.Update
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCompanyCommandHandler(
            IApplicationDbContext context
         )
        {
            _context = context;
        }


        bool IsValid(string? value)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.Trim().ToLower() != "string";
        }

        public async Task<bool> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(x => x.Id == command.CompanyId && x.IsActive, cancellationToken);

            if (company == null)
                return false;

            if (IsValid(command.Name) && command.Name != company.Name)
                company.Name = command.Name;

            if (IsValid(command.Description) && command.Description != company.Description)
                company.Description = command.Description;

            if (IsValid(command.Industry) && command.Industry != company.Industry)
                company.Industry = command.Industry;

            if (IsValid(command.Website) && command.Website != company.Website)
                company.Website = command.Website;

            if (IsValid(command.Location) && command.Location != company.Location)
                company.Location = command.Location;

            if (IsValid(command.Email) && command.Email != company.Email)
                company.Email = command.Email;

            if (IsValid(command.ContactPersonPhoneNumber) && command.ContactPersonPhoneNumber != company.ContactPersonPhoneNumber)
                company.ContactPersonPhoneNumber = command.ContactPersonPhoneNumber;

            if (IsValid(command.TaxIdentificationNumber) && command.TaxIdentificationNumber != company.TaxIdentificationNumber)
                company.TaxIdentificationNumber = command.TaxIdentificationNumber;

            if (IsValid(command.RegistrationNumber) && command.RegistrationNumber != company.RegistrationNumber)
                company.RegistrationNumber = command.RegistrationNumber;

            if (IsValid(command.OwnerName) && command.OwnerName != company.OwnerName)
                company.OwnerName = command.OwnerName;

            if (IsValid(command.ContactPersonFirstName) && command.ContactPersonFirstName != company.ContactPersonFirstName)
                company.ContactPersonFirstName = command.ContactPersonFirstName;

            if (IsValid(command.ContactPersonLastName) && command.ContactPersonLastName != company.ContactPersonLastName)
                company.ContactPersonLastName = command.ContactPersonLastName;

            if (IsValid(command.ContactPersonPosition) && command.ContactPersonPosition != company.ContactPersonPosition)
                company.ContactPersonPosition = command.ContactPersonPosition;

            if (IsValid(command.PasswordHash) && command.PasswordHash != company.PasswordHash)
                company.PasswordHash = command.PasswordHash;

            if (IsValid(command.PhoneNumber) && command.PhoneNumber != company.PhoneNumber)
                company.PhoneNumber = command.PhoneNumber;

            if (IsValid(command.Address) && command.Address != company.Address)
                company.Address = command.Address;

            if (IsValid(command.CompanySize) && command.CompanySize != company.CompanySize)
                company.CompanySize = command.CompanySize;

            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
