using JobLess.Advertisement.Domain.Enums;
using MediatR;

namespace JobLess.Advertisement.Application.Commands.Create
{
    public class CreateAdvertisementCommand : IRequest<int>
    {
        public required int CompanyId { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required string Position { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public EmploymentType EmploymentType { get; init; }
        public WorkSchedule WorkSchedule { get; init; }
        public SeniorityLevel SeniorityLevel { get; init; }
        public int? MinExperience { get; set; }
        public int? MaxExperience { get; set; }
        public required string City { get; init; }
        public string? Country { get; init; }
        public WorkType WorkType { get; init; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public string? Currency { get; set; }
        public bool IsSalaryVisible { get; set; } = true;
    }
}