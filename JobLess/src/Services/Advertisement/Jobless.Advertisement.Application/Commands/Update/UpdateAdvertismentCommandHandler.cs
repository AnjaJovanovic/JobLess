using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Advertisement.Application.Commands.Update
{
    public class UpdateAdvertisementCommandHandler
        : IRequestHandler<UpdateAdvertisementCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;

        public UpdateAdvertisementCommandHandler(
            IApplicationDbContext context,
            IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<Unit> Handle(
            UpdateAdvertisementCommand command,
            CancellationToken cancellationToken)
        {
            var advertisement = await _context.JobAdvertisements
                .Where(x => x.Id == command.Id && x.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (advertisement == null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id",
                        "Advertisement does not exist or has already been deactivated.");
                throw new InvalidOperationException();
            }

            if (command.Title != null && command.Title != advertisement.Title)
                advertisement.Title = command.Title;

            if (command.Description != null && command.Description != advertisement.Description)
                advertisement.Description = command.Description;

            if (command.Position != null && command.Position != advertisement.Position)
                advertisement.Position = command.Position;

            if (command.City != null && command.City != advertisement.City)
                advertisement.City = command.City;

            if (command.Country != null && command.Country != advertisement.Country)
                advertisement.Country = command.Country;

            if (command.Currency != null && command.Currency != advertisement.Currency)
                advertisement.Currency = command.Currency;

            if (command.ExpiresAt.HasValue && command.ExpiresAt.Value != advertisement.ExpiresAt)
                advertisement.ExpiresAt = command.ExpiresAt.Value;

            if (command.EmploymentType.HasValue && command.EmploymentType.Value != advertisement.EmploymentType)
                advertisement.EmploymentType = command.EmploymentType.Value;

            if (command.WorkSchedule.HasValue && command.WorkSchedule.Value != advertisement.WorkSchedule)
                advertisement.WorkSchedule = command.WorkSchedule.Value;

            if (command.SeniorityLevel.HasValue && command.SeniorityLevel.Value != advertisement.SeniorityLevel)
                advertisement.SeniorityLevel = command.SeniorityLevel.Value;

            if (command.WorkType.HasValue && command.WorkType.Value != advertisement.WorkType)
                advertisement.WorkType = command.WorkType.Value;

            if (command.MinExperience.HasValue && command.MinExperience.Value != advertisement.MinExperience)
                advertisement.MinExperience = command.MinExperience.Value;

            if (command.MaxExperience.HasValue && command.MaxExperience.Value != advertisement.MaxExperience)
                advertisement.MaxExperience = command.MaxExperience.Value;

            if (command.SalaryFrom.HasValue && command.SalaryFrom.Value != advertisement.SalaryFrom)
                advertisement.SalaryFrom = command.SalaryFrom.Value;

            if (command.SalaryTo.HasValue && command.SalaryTo.Value != advertisement.SalaryTo)
                advertisement.SalaryTo = command.SalaryTo.Value;

            if (command.IsSalaryVisible.HasValue && command.IsSalaryVisible.Value != advertisement.IsSalaryVisible)
                advertisement.IsSalaryVisible = command.IsSalaryVisible.Value;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}