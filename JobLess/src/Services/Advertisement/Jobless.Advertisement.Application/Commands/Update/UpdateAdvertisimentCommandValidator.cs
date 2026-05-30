using FluentValidation;

namespace JobLess.Advertisement.Application.Commands.Update
{
    public class UpdateAdvertisementCommandValidator : AbstractValidator<UpdateAdvertisementCommand>
    {
        public UpdateAdvertisementCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Naziv posla ne može biti prazan.")
                .MaximumLength(255)
                .WithMessage("Naziv može imati najviše 255 karaktera.")
                .When(x => x.Title != null);

            RuleFor(x => x.Position)
                .NotEmpty()
                .WithMessage("Pozicija ne može biti prazna.")
                .MaximumLength(255)
                .WithMessage("Naziv pozicije može imati najviše 255 karaktera.")
                .When(x => x.Position != null);

            RuleFor(x => x.Description)
                .MaximumLength(5000)
                .WithMessage("Opis može imati najviše 5000 karaktera.")
                .When(x => x.Description != null);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Grad ne može biti prazan.")
                .When(x => x.City != null);


            RuleFor(x => x.Country)
                .MaximumLength(100)
                .WithMessage("Naziv države može imati najviše 100 karaktera.")
                .When(x => x.Country != null);

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Datum isteka mora biti u budućnosti.")
                .When(x => x.ExpiresAt.HasValue);


            RuleFor(x => x.MinExperience)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimalno iskustvo ne može biti negativno.")
                .When(x => x.MinExperience.HasValue);


            RuleFor(x => x.MaxExperience)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maksimalno iskustvo ne može biti negativno.")
                .When(x => x.MaxExperience.HasValue);


            RuleFor(x => x)
                .Must(x => x.MinExperience <= x.MaxExperience)
                .WithMessage("Minimalno iskustvo ne može biti veće od maksimalnog.")
                .When(x => x.MinExperience.HasValue && x.MaxExperience.HasValue);


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
                .WithMessage("Valuta je obavezna kada je plata navedena.")
                .When(x => x.SalaryFrom.HasValue || x.SalaryTo.HasValue);

            RuleFor(x => x.Currency)
                .MaximumLength(10)
                .WithMessage("Valuta može imati najviše 10 karaktera.")
                .When(x => !string.IsNullOrWhiteSpace(x.Currency));


            RuleFor(x => x)
                .Must(x => x.SalaryFrom.HasValue || x.SalaryTo.HasValue)
                .WithMessage("Ako je plata označena kao vidljiva, mora biti unet bar jedan iznos.")
                .When(x => x.IsSalaryVisible == true);


            RuleFor(x => x.EmploymentType)
                .IsInEnum()
                .WithMessage("Nevažeći tip zaposlenja.")
                .When(x => x.EmploymentType.HasValue);

            RuleFor(x => x.WorkSchedule)
                .IsInEnum()
                .WithMessage("Nevažeći raspored rada.")
                .When(x => x.WorkSchedule.HasValue);

            RuleFor(x => x.SeniorityLevel)
                .IsInEnum()
                .WithMessage("Nevažeći nivo iskustva.")
                .When(x => x.SeniorityLevel.HasValue);

            RuleFor(x => x.WorkType)
                .IsInEnum()
                .WithMessage("Nevažeći tip rada.")
                .When(x => x.WorkType.HasValue);
        }
    }
}