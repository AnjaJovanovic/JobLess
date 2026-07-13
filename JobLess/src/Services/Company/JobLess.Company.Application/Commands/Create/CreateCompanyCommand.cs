using JobLess.Company.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Commands.Create
{
    public class CreateCompanyCommand : IRequest<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        //public required Industry Industry { get; set; }
        public required string Industry { get; set; }
        public string? Website { get; set; }
        public required string TaxIdentificationNumber { get; set; } //PIB
        public required string RegistrationNumber { get; set; } //maticni broj
        public string? OwnerName { get; set; }

        public required string ContactPersonFirstName { get; set; }
        public required string ContactPersonLastName { get; set; }
        public required string ContactPersonPosition { get; set; }
        public required string ContactPersonPhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public required string Location { get; set; }
        public required int OwnerId { get; set; }
       // public required int EmployeeCount { get; set; }
        public required CompanySize CompanySize { get; set; }
    }
}


