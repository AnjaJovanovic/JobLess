using JobLess.Oglasi.Application.Commands.IzbrisiOglas;
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
namespace JobLess.Oglasi.Application.Commands.AktivirajOglas
{
    public class AktivirajOglasHandler
        : IRequestHandler<AktivirajOglasCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;

        public AktivirajOglasHandler(
            IApplicationDbContext context,
            IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<bool> Handle(
            AktivirajOglasCommand command,
            CancellationToken cancellationToken)
        {
            var oglas = await _context.Oglasi
                .Where(x => x.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (oglas == null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id",
                        "Oglas ne postoji u bazi podataka.");
            }

            if (oglas.Aktivan)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Aktivan",
                        "Oglas je već aktivan.");
            }

            oglas.Aktivan = true;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
