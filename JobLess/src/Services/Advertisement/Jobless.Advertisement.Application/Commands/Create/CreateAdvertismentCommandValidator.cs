using FluentValidation;

namespace JobLess.Advertisement.Application.Commands.Create
{
    public class CreateAdvertisementCommandValidator : AbstractValidator<CreateAdvertisementCommand>
    {
        public CreateAdvertisementCommandValidator()
        {
            RuleFor(x => x.CompanyId)
                .GreaterThan(0)
                .WithMessage("Kompanija je obavezna.");

            RuleFor(x => x.CompanyEmail)
                .NotEmpty()
                .WithMessage("Kompanija nije autentifikovana.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Naziv posla je obavezan.")
                .MaximumLength(255)
                .WithMessage("Naziv može imati najviše 255 karaktera.");

            RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage("Pozicija je obavezna.")
                .MaximumLength(255)
                .WithMessage("Naziv pozicije može imati najviše 255 karaktera.");

            RuleFor(x => x.Description)
                .MaximumLength(5000)
                .WithMessage("Opis može imati najviše 5000 karaktera.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Grad je obavezan.");

            RuleFor(x => x.Country)
                .MaximumLength(100)
                .WithMessage("Naziv države može imati najviše 100 karaktera.");

            RuleFor(x => x.ExpiresAt)
                .Must(d => d == null || d > DateTime.UtcNow)
                .WithMessage("Datum isteka mora biti u budućnosti.");


            RuleFor(x => x.MinExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinExperience.HasValue)
                .WithMessage("Minimalno iskustvo ne može biti negativno.");

            RuleFor(x => x.MaxExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxExperience.HasValue)
                .WithMessage("Maksimalno iskustvo ne može biti negativno.");

            RuleFor(x => x)
                .Must(x => !x.MinExperience.HasValue ||
                           !x.MaxExperience.HasValue ||
                           x.MinExperience <= x.MaxExperience)
                .WithMessage("Minimalno iskustvo ne može biti veće od maksimalnog.");


            RuleFor(x => x.SalaryFrom)
                .GreaterThan(0)
                .When(x => x.SalaryFrom.HasValue)
                .WithMessage("Minimalna plata mora biti veća od 0.");

            RuleFor(x => x.SalaryTo)
                .GreaterThan(0)
                .When(x => x.SalaryTo.HasValue)
                .WithMessage("Maksimalna plata mora biti veća od 0.");

            RuleFor(x => x)
                .Must(x => !x.SalaryFrom.HasValue ||
                           !x.SalaryTo.HasValue ||
                           x.SalaryFrom <= x.SalaryTo)
                .WithMessage("Minimalna plata ne može biti veća od maksimalne plate.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .When(x => x.SalaryFrom.HasValue || x.SalaryTo.HasValue)
                .WithMessage("Valuta je obavezna kada je plata navedena.");

            RuleFor(x => x.Currency)
                .MaximumLength(10)
                .When(x => !string.IsNullOrWhiteSpace(x.Currency))
                .WithMessage("Valuta može imati najviše 10 karaktera.");

            RuleFor(x => x)
                .Must(x => !x.IsSalaryVisible ||
                           x.SalaryFrom.HasValue ||
                           x.SalaryTo.HasValue)
                .WithMessage("Ako je plata označena kao vidljiva, mora biti unos bar jedan iznos.");


            RuleFor(x => x.EmploymentType)
                .IsInEnum()
                .WithMessage("Nevažeći tip zaposlenja.");

            RuleFor(x => x.WorkSchedule)
                .IsInEnum()
                .WithMessage("Nevažeći raspored rada.");

            RuleFor(x => x.SeniorityLevel)
                .IsInEnum()
                .WithMessage("Nevažeći nivo iskustva.");

            RuleFor(x => x.WorkType)
                .IsInEnum()
                .WithMessage("Nevažeći tip rada.");
        }
    }
}