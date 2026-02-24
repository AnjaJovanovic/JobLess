using JobLess.Oglasi.Application.Interfaces;
using JobLess.Oglasi.Application.Models;
using JobLess.Oglasi.Application.Queries.VratiSveOglase;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Oglasi.Application.Queries.VratiOglas
{
    public class VratiOglasHandler : IRequestHandler<VratiOglasQuery, VratiOglasResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;


        public VratiOglasHandler(IApplicationDbContext context, IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<VratiOglasResult> Handle(VratiOglasQuery query, CancellationToken cancellationToken)
        {
            var oglas = await _context.Oglasi
            .Where(x => x.Id == query.Id && x.Aktivan == true)
            .Select(OglasModel.Projekcija)
            .FirstOrDefaultAsync(cancellationToken);

            if(oglas == null)
            {
                _validationExceptionThrower.ThrowValidationException("Id", "Oglas ne postoji ili nije aktivan.");
            }
            return new VratiOglasResult
            {
                Oglas = oglas
            };
        }
    }
}
