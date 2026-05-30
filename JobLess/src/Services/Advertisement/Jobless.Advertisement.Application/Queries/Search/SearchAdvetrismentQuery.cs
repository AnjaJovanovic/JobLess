using JobLess.Advertisement.Domain.Enums;
using MediatR;

namespace JobLess.Advertisement.Application.Queries.Search
{
    public class SearchAdvertisementQuery : IRequest<SearchAdvertisementResult>
    {
        public int? CompanyId { get; init; }
        public string? Title { get; init; }
        public string? Position { get; init; }
        public DateTime? ExpiresFrom { get; init; }
        public DateTime? ExpiresTo { get; init; }
        public EmploymentType? EmploymentType { get; init; }
        public WorkSchedule? WorkSchedule { get; init; }
        public SeniorityLevel? SeniorityLevel { get; init; }
        public int? MinExperience { get; init; }
        public int? MaxExperience { get; init; }
        public string? City { get; init; }
        public string? Country { get; init; }
        public WorkType? WorkType { get; init; }
        public decimal? SalaryFrom { get; init; }
        public decimal? SalaryTo { get; init; }
        public string? Currency { get; init; }
        public bool? IsSalaryVisible { get; init; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}