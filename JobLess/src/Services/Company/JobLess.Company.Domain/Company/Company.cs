using JobLess.Shared.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Domain.Entities
{
    public class Company : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Industry { get; set; }
        public string? Website { get; set; }
        public required string TaxIdentificationNumber { get; set; } //PIB
        public required string RegistrationNumber    { get; set; } //maticni broj
        public string OwnerName  { get; set; }

        public required string ContactPersonFirstName { get; set; }
        public required string ContactPersonLastName { get; set; }
        public required string ContactPersonPosition { get; set; }
        public required string ContactPersonPhoneNumber { get; set; }
        public required string Email {  get; set; }
        public required string PasswordHash  { get; set; }

        public string PhoneNumber   { get; set; }
        public required string Location { get; set; }
        public string Address { get; set; }
        public string CompanySize { get; set; }

        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
