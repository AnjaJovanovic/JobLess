using FluentValidation;
using JobLess.Company.Application.Common.Helpers;
using JobLess.Company.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Commands.Create
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        /*private static readonly string[] AllowedIndustries =
                {
                    "Informacione tehnologije",
                    "Finansije i bankarstvo",
                    "Maloprodaja i usluge",
                    "Industrija i proizvodnja",
                    "Zdravstvo",
                    "Građevinarstvo",
                    "Mediji i marketing",
                    "Ostalo"
                };

        private static readonly string[] AllowedCompanySizes =
            {
                "1-10",
                "11-50",
                "51-200",
                "201-500",
                "500+"
            };*/
        public CreateCompanyCommandValidator()
        {

            RuleFor(x => x.Name)
         .NotEmpty().WithMessage("Ime kompanije je obavezno polje.")
         .MaximumLength(200).WithMessage("Ime kompanije ne sme da ima više od 200 karaktera.");


            /*RuleFor(x => x.Industry)
                .NotEmpty()
                .WithMessage("Delatnost je obavezno polje.")
                .IsInEnum()
                .WithMessage("Odabrana delatnost nije validna.");*/

            RuleFor(x => x.Industry)
                .Must(IndustryHelper.IsValid)
                .WithMessage("Odabrana industrija nije validna.");

            RuleFor(x => x.CompanySize)
                 .Must(x => Enum.IsDefined(typeof(CompanySize), x))
                 .WithMessage("Veličina kompanije je obavezno i mora biti validna.");

            /* RuleFor(x => x.EmployeeCount)
                 .GreaterThan(0)
                 .WithMessage("Broj zaposlenih mora biti veći od 0.");*/

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Lokacija je obavezno polje.")
                .MaximumLength(200).WithMessage("Lokacija ne sme da ima više od 200 karaktera.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Opis ne sme da ima više od 2000 karaktera.");

            RuleFor(x => x.Website)
                .MaximumLength(300).WithMessage("Veb sajt ne sme da ima više od 300 karaktera.")
                .Must(BeValidUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.Website))
                .WithMessage("Unesite ispravan URL (npr. www.kompanija.rs).");
            /*
            RuleFor(x => x.OwnerId)
                .GreaterThan(0)
                .WithMessage("Vlasnik je obavezno polje.");
            */

            RuleFor(x => x.TaxIdentificationNumber)
                .NotEmpty().WithMessage("PIB je obavezno polje.")
                .Matches(@"^\d{9}$")
                .WithMessage("PIB mora imati tačno 9 cifara.");

            RuleFor(x => x.RegistrationNumber)
                .NotEmpty().WithMessage("Matični broj je obavezno polje.")
                .Matches(@"^\d{8}$")
                .WithMessage("Matični broj mora imati tačno 8 cifara.");

            RuleFor(x => x.ContactPersonFirstName)
                .NotEmpty().WithMessage("Ime kontakt osobe je obavezno polje.")
                .MaximumLength(30).WithMessage("Ime kontakt osobe ne sme da ima više od 30 karaktera.");

            RuleFor(x => x.ContactPersonLastName)
                .NotEmpty().WithMessage("Prezime kontakt osobe je obavezno polje.")
                .MaximumLength(30).WithMessage("Prezime kontakt osobe ne sme da ima više od 30 karaktera.");

            RuleFor(x => x.ContactPersonPosition)
                .NotEmpty().WithMessage("Pozicija kontakt osobe je obavezno polje.")
                .MaximumLength(50).WithMessage("Pozicija kontakt osobe ne sme da ima više od 50 karaktera.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email je obavezno polje.")
                .EmailAddress().WithMessage("Unesite ispravnu email adresu.")
                .MaximumLength(50).WithMessage("Email ne sme da ima više od 50 karaktera.");

            RuleFor(x => x.ContactPersonPhoneNumber)
                .NotEmpty().WithMessage("Telefon kontakt osobe je obavezno polje.")
                .Matches(@"^\+[0-9][0-9\s\-()/]{5,18}$")
                .WithMessage("Broj telefona mora biti u formatu +381 60 123 4567.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+[0-9][0-9\s\-()/]{5,18}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Broj telefona mora biti u formatu +381 60 123 4567.");

            RuleFor(x => x.PasswordHash)
                .NotEmpty().WithMessage("Šifra je obavezno polje.")
                .MinimumLength(8).WithMessage("Lozinka mora imati najmanje 8 karaktera.")
                .Matches("[A-Z]").WithMessage("Lozinka mora sadržati bar jedno veliko slovo.")
                .Matches("[0-9]").WithMessage("Lozinka mora sadržati bar jedan broj.")
                .MaximumLength(100).WithMessage("Šifra ne sme da ima više od 100 karaktera.");
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
