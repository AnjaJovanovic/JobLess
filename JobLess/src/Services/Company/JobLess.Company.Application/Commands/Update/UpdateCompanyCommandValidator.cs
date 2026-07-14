using FluentValidation;
using JobLess.Company.Application.Common.Helpers;

namespace JobLess.Company.Application.Commands.Update
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyCommandValidator()
        {

            RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Ime kompanije ne sme da ima više od 200 karaktera.");

            RuleFor(x => x.Industry)
                .IsInEnum()
                .When(x => x.Industry.HasValue)
                .WithMessage("Odabrana delatnost nije validna.");

            RuleFor(x => x.EmployeeCount)
                .GreaterThan(0)
                .When(x => x.EmployeeCount.HasValue)
                .WithMessage("Broj zaposlenih mora biti veći od 0.");

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .WithMessage("Lokacija ne sme da ima više od 200 karaktera.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Opis ne sme da ima više od 2000 karaktera.");

            RuleFor(x => x.Website)
                .MaximumLength(300)
                .WithMessage("Veb sajt ne sme da ima više od 300 karaktera.")
                .Must(BeValidUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.Website))
                .WithMessage("Unesite ispravan URL (npr. www.kompanija.rs).");

            RuleFor(x => x.ContactPersonFirstName)
                .MaximumLength(30)
                .WithMessage("Ime kontakt osobe ne sme da ima više od 30 karaktera.");

            RuleFor(x => x.ContactPersonLastName)
                .MaximumLength(30)
                .WithMessage("Prezime kontakt osobe ne sme da ima više od 30 karaktera.");

            RuleFor(x => x.ContactPersonPosition)
                .MaximumLength(50)
                .WithMessage("Pozicija kontakt osobe ne sme da ima više od 50 karaktera.");

            RuleFor(x => x.ContactPersonPhoneNumber)
                .Matches(@"^\+[0-9][0-9\s\-()/]{5,18}$")
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPersonPhoneNumber))
                .WithMessage("Broj telefona mora biti u formatu +381 60 123 4567.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+[0-9][0-9\s\-()/]{5,18}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Broj telefona mora biti u formatu +381 60 123 4567.");
        }
        
        private static bool BeValidUrl(string? website)
        {
            if (string.IsNullOrWhiteSpace(website))
                return true;

            var url = website.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? website
                : $"https://{website}";

            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
