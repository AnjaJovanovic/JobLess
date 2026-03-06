using System;
using System.Linq.Expressions;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;


namespace JobLess.Advertisement.Application.Models
{
    public class AdvertisementModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Position { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public WorkSchedule WorkSchedule { get; set; }
        public SeniorityLevel SeniorityLevel { get; set; }
        public int? MinExperience { get; set; }
        public int? MaxExperience { get; set; }
        public string City { get; set; } = string.Empty;
        public string? Country { get; set; }
        public WorkType WorkType { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public string? Currency { get; set; }
        public bool IsSalaryVisible { get; set; }

        public static Expression<Func<JobAdvertisement, AdvertisementModel>> Projection
        {
            get
            {
                return entity => new AdvertisementModel
                {
                    Id = entity.Id,
                    CompanyId = entity.CompanyId,
                    Title = entity.Title,
                    Description = entity.Description,
                    Position = entity.Position,
                    PostedAt = entity.PostedAt,
                    ExpiresAt = entity.ExpiresAt,
                    IsActive = entity.IsActive,
                    EmploymentType = entity.EmploymentType,
                    WorkSchedule = entity.WorkSchedule,
                    SeniorityLevel = entity.SeniorityLevel,
                    MinExperience = entity.MinExperience,
                    MaxExperience = entity.MaxExperience,
                    City = entity.City,
                    Country = entity.Country,
                    WorkType = entity.WorkType,
                    SalaryFrom = entity.SalaryFrom,
                    SalaryTo = entity.SalaryTo,
                    Currency = entity.Currency,
                    IsSalaryVisible = entity.IsSalaryVisible
                };
            }
        }
    }
}

