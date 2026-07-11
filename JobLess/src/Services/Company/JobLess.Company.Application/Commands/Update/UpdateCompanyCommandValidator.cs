using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Commands.Update
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
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
        public UpdateCompanyCommandValidator()
        {

            RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Ime kompanije ne sme da ima više od 200 karaktera.");

            /* RuleFor(x => x.CompanySize)
                 .IsInEnum()
                 .WithMessage("Odabrana veličina kompanije nije validna.");*/

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

            RuleFor(x => x.PasswordHash)
                .MinimumLength(8)
                .WithMessage("Lozinka mora imati najmanje 8 karaktera.")
                .Matches("[A-Z]")
                .WithMessage("Lozinka mora sadržati bar jedno veliko slovo.")
                .Matches("[0-9]")
                .WithMessage("Lozinka mora sadržati bar jedan broj.")
                .MaximumLength(100)
                .WithMessage("Šifra ne sme da ima više od 100 karaktera.")
                .When(x => !string.IsNullOrWhiteSpace(x.PasswordHash));
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
