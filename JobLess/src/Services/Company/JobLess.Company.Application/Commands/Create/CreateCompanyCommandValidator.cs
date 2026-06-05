using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Application.Commands.Create
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Ime kompanije je obavezno polje.")
                .MaximumLength(200)
                .WithMessage("Ime kompanije ne sme da ima više od 200 karaktera.");

            RuleFor(x => x.Industry)
                .NotEmpty()
                .WithMessage("Delatnost je obavezno polje.")
                .MaximumLength(100)
                .WithMessage("Delatnost ne sme da ima više od 100 karaktera");

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage("Lokacija je obavezno polje")
                .MaximumLength(200)
                .WithMessage("Lokacija ne sme da ima više od 200 karaktera");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Opis ne sme da ima više od 2000 karaktera.");

            RuleFor(x => x.Website)
                .MaximumLength(300)
                .WithMessage("Veb sajt ne sme da ima više od 300 karaktera.");

            RuleFor(x => x.OwnerId)
                .GreaterThan(0)
                .WithMessage("Vlasnik je obavezno polje.");

            RuleFor(x => x.TaxIdentificationNumber)
                .NotEmpty()
                .WithMessage("PIB je obavezno polje.")
                .MaximumLength(50)
                .WithMessage("PIB ne sme da ima više od 50 karaktera.");

            RuleFor(x => x.RegistrationNumber)
                .NotEmpty()
                .WithMessage("Matični broj je obavezno polje.")
                .MaximumLength(50)
                .WithMessage("Matični broj ne sme da ima više od 50 karaktera");

            RuleFor(x => x.ContactPersonFirstName)
                .NotEmpty()
                .WithMessage("Ime kontakt osobe je obavezno polje.")
                .MaximumLength(30)
                .WithMessage("Ime kontakt osobe ne sme da ima više od 30 karaktera");

            RuleFor(x => x.ContactPersonLastName)
                .NotEmpty()
                .WithMessage("Prezime kontakt osobe je obavezno polje.")
                .MaximumLength(30)
                .WithMessage("Prezime kontakt osobe ne sme da ima više od 30 karaktera");

            RuleFor(x => x.ContactPersonPosition)
                .NotEmpty()
                .WithMessage("Pozicija kontakt osobe je obavezno polje.")
                .MaximumLength(50)
                .WithMessage("Pozicija kontakt osobe ne sme da ima više od 50 karaktera");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email je obavezno polje.")
                .MaximumLength(50)
                .WithMessage("Email ne sme da ima više od 50 karaktera");

            RuleFor(x => x.PasswordHash)
               .NotEmpty()
               .WithMessage("Šifra je obavezno polje")
               .MaximumLength(100)
               .WithMessage("Šifra ne sme da ima više od 100 karaktera");
        }

    }
}
