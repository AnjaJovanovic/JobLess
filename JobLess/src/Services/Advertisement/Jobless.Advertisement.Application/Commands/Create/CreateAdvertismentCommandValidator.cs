using FluentValidation;

namespace JobLess.Advertisement.Application.Commands.Create
{
    public class CreateAdvertisementCommandValidator : AbstractValidator<CreateAdvertisementCommand>
    {
        public CreateAdvertisementCommandValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0)
                .WithMessage("Company is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Job title is required.")
                .MaximumLength(255)
                .WithMessage("Title can have at most 255 characters.");

            RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage("Position is required.")
                .MaximumLength(255)
                .WithMessage("Position name can have at most 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(5000)
                .WithMessage("Description can have at most 5000 characters.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(x => x.Country)
                .MaximumLength(100)
                .WithMessage("Country name can have at most 100 characters.");

            RuleFor(x => x.ExpiresAt)
                .Must(d => d == null || d > DateTime.UtcNow)
                .WithMessage("Expiration date must be in the future.");

            // ===================== EXPERIENCE =====================

            RuleFor(x => x.MinExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinExperience.HasValue)
                .WithMessage("Minimum experience cannot be negative.");

            RuleFor(x => x.MaxExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxExperience.HasValue)
                .WithMessage("Maximum experience cannot be negative.");

            RuleFor(x => x)
                .Must(x => !x.MinExperience.HasValue ||
                           !x.MaxExperience.HasValue ||
                           x.MinExperience <= x.MaxExperience)
                .WithMessage("Minimum experience cannot be greater than maximum.");

            // ===================== SALARY =====================

            RuleFor(x => x.SalaryFrom)
                .GreaterThan(0)
                .When(x => x.SalaryFrom.HasValue)
                .WithMessage("Salary from must be greater than 0.");

            RuleFor(x => x.SalaryTo)
                .GreaterThan(0)
                .When(x => x.SalaryTo.HasValue)
                .WithMessage("Salary to must be greater than 0.");

            RuleFor(x => x)
                .Must(x => !x.SalaryFrom.HasValue ||
                           !x.SalaryTo.HasValue ||
                           x.SalaryFrom <= x.SalaryTo)
                .WithMessage("Salary from cannot be greater than salary to.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .When(x => x.SalaryFrom.HasValue || x.SalaryTo.HasValue)
                .WithMessage("Currency is required when salary is provided.");

            RuleFor(x => x.Currency)
                .MaximumLength(10)
                .When(x => !string.IsNullOrWhiteSpace(x.Currency))
                .WithMessage("Currency can have at most 10 characters.");

            RuleFor(x => x)
                .Must(x => !x.IsSalaryVisible ||
                           x.SalaryFrom.HasValue ||
                           x.SalaryTo.HasValue)
                .WithMessage("If salary is marked as visible, at least one salary amount must be provided.");

            // ===================== ENUM VALIDATIONS =====================

            RuleFor(x => x.EmploymentType)
                .IsInEnum()
                .WithMessage("Invalid employment type.");

            RuleFor(x => x.WorkSchedule)
                .IsInEnum()
                .WithMessage("Invalid work schedule.");

            RuleFor(x => x.SeniorityLevel)
                .IsInEnum()
                .WithMessage("Invalid seniority level.");

            RuleFor(x => x.WorkType)
                .IsInEnum()
                .WithMessage("Invalid work type.");
        }
    }
}