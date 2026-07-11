using FluentValidation;

namespace JobLess.Client.Application.Clients.Commands.CreateClientProfile;

public class CreateClientProfileCommandValidator : AbstractValidator<CreateClientProfileCommand>
{
    public CreateClientProfileCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .Matches(@"^\+[0-9][0-9\s\-()/]{5,18}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Broj telefona mora biti u formatu +381 60 123 4567.");

        RuleFor(x => x.Gender)
            .IsInEnum();

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob is null || dob.Value.Date < DateTime.UtcNow.Date)
            .WithMessage("Datum rođenja mora biti u prošlosti.");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => x.City is not null);

        RuleFor(x => x.Address)
            .MaximumLength(250)
            .When(x => x.Address is not null);

        RuleFor(x => x.EducationLevel)
            .IsInEnum()
            .When(x => x.EducationLevel.HasValue);

        RuleFor(x => x.InstitutionName)
            .MaximumLength(200)
            .When(x => x.InstitutionName is not null);

        RuleFor(x => x.EducationStartYear)
            .InclusiveBetween(1950, DateTime.UtcNow.Year + 1)
            .When(x => x.EducationStartYear.HasValue);

        RuleFor(x => x.EducationEndYear)
            .InclusiveBetween(1950, DateTime.UtcNow.Year + 1)
            .When(x => x.EducationEndYear.HasValue);

        RuleFor(x => x)
            .Must(x => !x.EducationStartYear.HasValue || !x.EducationEndYear.HasValue
                || x.EducationStartYear.Value <= x.EducationEndYear.Value)
            .WithMessage("Godina završetka ne može biti pre godine početka.");

        RuleFor(x => x.YearsOfExperience)
            .InclusiveBetween(0, 60)
            .When(x => x.YearsOfExperience.HasValue);

        RuleFor(x => x.Skills)
            .MaximumLength(1000)
            .When(x => x.Skills is not null);

        RuleFor(x => x.ProfessionalSummary)
            .MaximumLength(2000)
            .When(x => x.ProfessionalSummary is not null);

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(500)
            .When(x => x.LinkedInUrl is not null);
    }
}
