using JobLess.Advertisement.Domain.Enums;
using MediatR;

namespace JobLess.Advertisement.Application.Commands.Update
{
    public class UpdateAdvertisementCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Position { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public WorkSchedule? WorkSchedule { get; set; }
        public SeniorityLevel? SeniorityLevel { get; set; }
        public int? MinExperience { get; set; }
        public int? MaxExperience { get; set; }
        public string? City { get; init; }
        public string? Country { get; init; }
        public WorkType? WorkType { get; init; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public string? Currency { get; set; }
        public bool? IsSalaryVisible { get; set; }
    }
}