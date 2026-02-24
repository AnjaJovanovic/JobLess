using JobLess.Oglasi.Application.Commands.KreirajOglas;
using JobLess.Oglasi.Application.Interfaces;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Oglasi.Application.Commands.IzbrisiOglas
{
    public class IzbrisiOglasHandler
        : IRequestHandler<IzbrisiOglasCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;

        public IzbrisiOglasHandler(
            IApplicationDbContext context,
            IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<bool> Handle(
            IzbrisiOglasCommand command,
            CancellationToken cancellationToken)
        {
            // soft delete -> deaktiviranje oglasa
            var oglas = await _context.Oglasi
                .Where(x => x.Id == command.Id && x.Aktivan)
                .FirstOrDefaultAsync(cancellationToken);

            if (oglas == null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id",
                        "Oglas ne postoji ili je već deaktiviran.");
            }

            oglas.Aktivan = false;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
