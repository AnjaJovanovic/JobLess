using JobLess.Oglasi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using JobLess.Oglasi.Application.Interfaces;
using Azure;
using JobLess.Oglasi.Domain.Entities;

namespace JobLess.Oglasi.Application.Commands.KreirajOglas
{
    public class KreirajOglasCommandHandler
     : IRequestHandler<KreirajOglasCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public KreirajOglasCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(
            KreirajOglasCommand request,
            CancellationToken cancellationToken)
        {
            var oglas = new Oglas
            {
                KompanijaId = request.KompanijaId,
                Naslov = request.Naslov,
                Opis = request.Opis,
                Pozicija = request.Pozicija,

                Status = StatusOglasa.Draft,
                DatumPostavljanja = DateTime.UtcNow,
                DatumIsteka = request.DatumIsteka,

                TipZaposlenja = request.TipZaposlenja,
                RadnoVreme = request.RadnoVreme,
                Senioritet = request.Senioritet,

                Grad = request.Grad,
                Drzava = request.Drzava,
                TipRada = request.TipRada,
                Aktivan = true,
                PlataOd = (decimal?)request.PlataOd,
                PlataDo = (decimal?)request.PlataDo,
                PlataVidljiva = request.PlataVidljiva,
                IskustvoMin = request.IskustvoMin,
                IskustvoMax = request.IskustvoMax
            };

            _context.Oglasi.Add(oglas);
            await _context.SaveChangesAsync(cancellationToken);

            return oglas.Id;
          
        }
    }
}
