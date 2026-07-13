using JobLess.Company.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Commands.Update
{
    public class UpdateCompanyCommand : IRequest<bool>
    {
        public required int CompanyId { get; set; }
        public string CompanyEmail { get; set; } = string.Empty;

        public string? Name { get; set; }
        public string? Description { get; set; }
        //public Industry? Industry { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        //public string? TaxIdentificationNumber { get; set; } //PIB
        //public string? RegistrationNumber { get; set; } //maticni broj
        public string? OwnerName { get; set; }

        public string? ContactPersonFirstName { get; set; }
        public string? ContactPersonLastName { get; set; }
        public string? ContactPersonPosition { get; set; }
        public string? ContactPersonPhoneNumber { get; set; }
        //public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public int? EmployeeCount { get; set; }
        // public CompanySize? CompanySize { get; set; }
    }
}

