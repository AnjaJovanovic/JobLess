using Azure.Core;
using JobLess.Oglasi.Application.Interfaces;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace JobLess.Oglasi.Application.Commands.AzurirajOglas
{
    public class AzurirajOglasHandler
        : IRequestHandler<AzurirajOglasCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;

        public AzurirajOglasHandler(IApplicationDbContext context, IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<Unit> Handle(
            AzurirajOglasCommand command,
            CancellationToken cancellationToken)
        {
            var oglas = await _context.Oglasi
                .Where(x => x.Id == command.Id && x.Aktivan)
                .FirstOrDefaultAsync(cancellationToken);

            if (oglas == null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id",
                        "Oglas ne postoji ili je već deaktiviran.");
                throw new InvalidOperationException();
            }

            if (command.Naslov != null && command.Naslov != oglas.Naslov)
                oglas.Naslov = command.Naslov;

            if (command.Opis != null && command.Opis != oglas.Opis)
                oglas.Opis = command.Opis;

            if (command.Pozicija != null && command.Pozicija != oglas.Pozicija)
                oglas.Pozicija = command.Pozicija;

            if (command.Grad != null && command.Grad != oglas.Grad)
                oglas.Grad = command.Grad;

            if (command.Drzava != null && command.Drzava != oglas.Drzava)
                oglas.Drzava = command.Drzava;

            if (command.Valuta != null && command.Valuta != oglas.Valuta)
                oglas.Valuta = command.Valuta;


            if (command.DatumIsteka.HasValue && command.DatumIsteka.Value != oglas.DatumIsteka)
                oglas.DatumIsteka = command.DatumIsteka.Value;

            if (command.TipZaposlenja.HasValue && command.TipZaposlenja.Value != oglas.TipZaposlenja)
                oglas.TipZaposlenja = command.TipZaposlenja.Value;

            if (command.RadnoVreme.HasValue && command.RadnoVreme.Value != oglas.RadnoVreme)
                oglas.RadnoVreme = command.RadnoVreme.Value;

            if (command.Senioritet.HasValue && command.Senioritet.Value != oglas.Senioritet)
                oglas.Senioritet = command.Senioritet.Value;

            if (command.Status.HasValue && command.Status.Value != oglas.Status)
                oglas.Status = command.Status.Value;

            if (command.TipRada.HasValue && command.TipRada.Value != oglas.TipRada)
                oglas.TipRada = command.TipRada.Value;
            if (command.IskustvoMin.HasValue && command.IskustvoMin.Value != oglas.IskustvoMin)
                oglas.IskustvoMin = command.IskustvoMin.Value;

            if (command.IskustvoMax.HasValue && command.IskustvoMax.Value != oglas.IskustvoMax)
                oglas.IskustvoMax = command.IskustvoMax.Value;

            if (command.PlataOd.HasValue && command.PlataOd.Value != oglas.PlataOd)
                oglas.PlataOd = command.PlataOd.Value;

            if (command.PlataDo.HasValue && command.PlataDo.Value != oglas.PlataDo)
                oglas.PlataDo = command.PlataDo.Value;


            if (command.PlataVidljiva.HasValue && command.PlataVidljiva.Value != oglas.PlataVidljiva)
                oglas.PlataVidljiva = command.PlataVidljiva.Value;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
