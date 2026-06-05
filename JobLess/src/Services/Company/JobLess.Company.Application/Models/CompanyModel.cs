using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TaxIdentificationNumber { get; set; } //PIB
        public string RegistrationNumber { get; set; } //maticni broj
        public string OwnerName { get; set; }

        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string Industry { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string Location { get; set; } = string.Empty;

        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string CompanySize { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    

        public static Expression<Func<Domain.Entities.Company, CompanyModel>> Projection
        {
            get
            {
                return entity => new CompanyModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    TaxIdentificationNumber = entity.TaxIdentificationNumber,
                    RegistrationNumber = entity.RegistrationNumber,
                    OwnerName = entity.OwnerName,
                    ContactPersonFirstName = entity.ContactPersonFirstName,
                    ContactPersonLastName = entity.ContactPersonLastName,
                    ContactPersonPhoneNumber = entity.ContactPersonPhoneNumber,
                    ContactPersonPosition = entity.ContactPersonPosition,
                    CompanySize = entity.CompanySize,
                    Address = entity.Address,
                    Email = entity.Email,
                    PasswordHash = entity.PasswordHash,
                    PhoneNumber = entity.PhoneNumber,
                    Industry = entity.Industry,
                    Website = entity.Website,
                    Location = entity.Location,
                    LogoUrl = entity.LogoUrl,
                    IsActive = entity.IsActive,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                };
            }
        }
    }
}
