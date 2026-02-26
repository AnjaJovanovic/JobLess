using MediatR;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using JobLess.Advertisement.Application.Interfaces;

namespace JobLess.Advertisement.Application.Commands.Create
{
    public class CreateAdvertisementCommandHandler
        : IRequestHandler<CreateAdvertisementCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateAdvertisementCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(
            CreateAdvertisementCommand request,
            CancellationToken cancellationToken)
        {
            var advertisement = new JobAdvertisement
            {
                CompanyId = request.CompanyId,
                Title = request.Title,
                Description = request.Description,
                Position = request.Position,
                Status = JobPostingStatus.Draft,
                PostedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpiresAt,
                EmploymentType = request.EmploymentType,
                WorkSchedule = request.WorkSchedule,
                SeniorityLevel = request.SeniorityLevel,
                City = request.City,
                Country = request.Country,
                WorkType = request.WorkType,
                IsActive = true,
                SalaryFrom = request.SalaryFrom,
                SalaryTo = request.SalaryTo,
                IsSalaryVisible = request.IsSalaryVisible,
                MinExperience = request.MinExperience,
                MaxExperience = request.MaxExperience
            };

            _context.JobAdvertisements.Add(advertisement);
            await _context.SaveChangesAsync(cancellationToken);

            return advertisement.Id;
        }
    }
}