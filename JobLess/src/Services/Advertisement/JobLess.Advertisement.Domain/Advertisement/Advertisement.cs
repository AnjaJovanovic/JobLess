using JobLess.Advertisement.Domain.Enums;
using JobLess.Shared.Domain.Common;
namespace JobLess.Advertisement.Domain.Entities;

public class JobAdvertisement : BaseEntity
{
    public int CompanyId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Position { get; set; }
    public required JobPostingStatus Status { get; set; }
    public required DateTime PostedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public required bool IsActive { get; set; }
    public required EmploymentType EmploymentType { get; set; }
    public required WorkSchedule WorkSchedule { get; set; }
    public required SeniorityLevel SeniorityLevel { get; set; }
    public int? MinExperience { get; set; }
    public int? MaxExperience { get; set; }
    public required string City { get; set; }
    public string? Country { get; set; }
    public required WorkType WorkType { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public string? Currency { get; set; }
    public bool IsSalaryVisible { get; set; } = true;
}