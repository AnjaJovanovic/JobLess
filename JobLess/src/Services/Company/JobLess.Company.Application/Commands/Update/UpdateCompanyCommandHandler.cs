using JobLess.Company.Application.Interfaces;
using JobLess.Company.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

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


        bool IsValid([NotNullWhen(true)] string? value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.Trim().ToLower() != "string";
        }

        public async Task<bool> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(x => x.Id == command.CompanyId && x.Email == command.CompanyEmail && x.IsActive, cancellationToken);

            if (company == null)
                return false;

            if (IsValid(command.Name) && command.Name != company.Name)
                company.Name = command.Name;

            if (IsValid(command.Description) && command.Description != company.Description)
                company.Description = command.Description;

            if (IsValid(command.Website) && command.Website != company.Website)
                company.Website = command.Website;

            if (IsValid(command.Location) && command.Location != company.Location)
                company.Location = command.Location;

            if (IsValid(command.ContactPersonPhoneNumber) && command.ContactPersonPhoneNumber != company.ContactPersonPhoneNumber)
                company.ContactPersonPhoneNumber = command.ContactPersonPhoneNumber;

            if (IsValid(command.OwnerName) && command.OwnerName != company.OwnerName)
                company.OwnerName = command.OwnerName;

            if (IsValid(command.ContactPersonFirstName) && command.ContactPersonFirstName != company.ContactPersonFirstName)
                company.ContactPersonFirstName = command.ContactPersonFirstName;

            if (IsValid(command.ContactPersonLastName) && command.ContactPersonLastName != company.ContactPersonLastName)
                company.ContactPersonLastName = command.ContactPersonLastName;

            if (IsValid(command.ContactPersonPosition) && command.ContactPersonPosition != company.ContactPersonPosition)
                company.ContactPersonPosition = command.ContactPersonPosition;

            if (IsValid(command.PhoneNumber) && command.PhoneNumber != company.PhoneNumber)
                company.PhoneNumber = command.PhoneNumber;

            if (IsValid(command.Address) && command.Address != company.Address)
                company.Address = command.Address;

            if (command.EmployeeCount.HasValue)
            {
                company.CompanySize = GetCompanySize(command.EmployeeCount.Value);
            }

            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
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
