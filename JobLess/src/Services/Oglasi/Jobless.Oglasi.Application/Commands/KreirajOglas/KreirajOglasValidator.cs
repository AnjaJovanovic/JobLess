using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;



namespace JobLess.Oglasi.Application.Commands.KreirajOglas
{
    public class KreirajOglasCommandValidator:AbstractValidator<KreirajOglasCommand>
    {
        public KreirajOglasCommandValidator()
        {
            RuleFor(x => x.KompanijaId)
             .GreaterThan(0)
             .WithMessage("Kompanija je obavezna.");

            RuleFor(x => x.Naslov)
                .NotEmpty()
                .WithMessage("Naslov oglasa je obavezan.")
                .MaximumLength(255)
                .WithMessage("Naslov može imati najviše 255 karaktera.");

            RuleFor(x => x.Pozicija)
                .NotEmpty()
                .WithMessage("Pozicija je obavezna.")
                .MaximumLength(255)
                .WithMessage("Naziv pozicije može imati najviše 255 karaktera.");

            RuleFor(x => x.Opis)
                .MaximumLength(5000)
                .WithMessage("Opis može imati najviše 5000 karaktera.");

            RuleFor(x => x.Grad)
                .NotEmpty()
                .WithMessage("Grad je obavezan.");

            RuleFor(x => x.Drzava)
                .MaximumLength(100)
                .WithMessage("Naziv države može imati najviše 100 karaktera.");

            RuleFor(x => x.DatumIsteka)
                .Must(d => d == null || d > DateTime.UtcNow)
                .WithMessage("Datum isteka mora biti u budućnosti.");

            // ===================== ISKUSTVO =====================

            RuleFor(x => x.IskustvoMin)
                .GreaterThanOrEqualTo(0)
                .When(x => x.IskustvoMin.HasValue)
                .WithMessage("Minimalno iskustvo ne može biti negativno.");

            RuleFor(x => x.IskustvoMax)
                .GreaterThanOrEqualTo(0)
                .When(x => x.IskustvoMax.HasValue)
                .WithMessage("Maksimalno iskustvo ne može biti negativno.");

            RuleFor(x => x)
                .Must(x => !x.IskustvoMin.HasValue ||
                           !x.IskustvoMax.HasValue ||
                           x.IskustvoMin <= x.IskustvoMax)
                .WithMessage("Minimalno iskustvo ne može biti veće od maksimalnog.");

            // ===================== PLATA =====================

            RuleFor(x => x.PlataOd)
                .GreaterThan(0)
                .When(x => x.PlataOd.HasValue)
                .WithMessage("Plata od mora biti veća od 0.");

            RuleFor(x => x.PlataDo)
                .GreaterThan(0)
                .When(x => x.PlataDo.HasValue)
                .WithMessage("Plata do mora biti veća od 0.");

            RuleFor(x => x)
                .Must(x => !x.PlataOd.HasValue ||
                           !x.PlataDo.HasValue ||
                           x.PlataOd <= x.PlataDo)
                .WithMessage("Plata od ne može biti veća od plate do.");

            // Ako je uneta bilo koja plata → valuta je obavezna
            RuleFor(x => x.Valuta)
                .NotEmpty()
                .When(x => x.PlataOd.HasValue || x.PlataDo.HasValue)
                .WithMessage("Valuta je obavezna kada je uneta plata.");

            RuleFor(x => x.Valuta)
                .MaximumLength(10)
                .When(x => !string.IsNullOrWhiteSpace(x.Valuta))
                .WithMessage("Valuta može imati najviše 10 karaktera.");

            // Ako je plata vidljiva → mora postojati bar jedna vrednost
            RuleFor(x => x)
                .Must(x => !x.PlataVidljiva.HasValue ||
                           !x.PlataVidljiva.Value ||
                           x.PlataOd.HasValue ||
                           x.PlataDo.HasValue)
                .WithMessage("Ako je plata označena kao vidljiva, mora biti unet iznos plate.");

            // ===================== ENUM VALIDACIJE =====================

            RuleFor(x => x.TipZaposlenja)
                .IsInEnum()
                .WithMessage("Neispravan tip zaposlenja.");

            RuleFor(x => x.RadnoVreme)
                .IsInEnum()
                .WithMessage("Neispravno radno vreme.");

            RuleFor(x => x.Senioritet)
                .IsInEnum()
                .WithMessage("Neispravan senioritet.");

            RuleFor(x => x.TipRada)
                .IsInEnum()
                .WithMessage("Neispravan tip rada.");
        }



    }
    }
