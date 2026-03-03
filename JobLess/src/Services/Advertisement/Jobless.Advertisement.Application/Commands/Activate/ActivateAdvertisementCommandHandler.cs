using JobLess.Advertisement.Application.Commands.Activate;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Commands.Delete;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace JobLess.Advertisement.Application.Commands.Activate
{
    public class AktivirajOglasHandler
        : IRequestHandler<ActivateAdvertisementCommand, bool>
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
            ActivateAdvertisementCommand command,
            CancellationToken cancellationToken)
        {
            var advertisement = await _context.JobAdvertisements
                .Where(x => x.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (advertisement == null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id",
                        "Advertisement don't exists.");
            }
            /*
            if (advertisement!.IsActive == true)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Active",
                        "Advertisement is already active.");
            }
            */
            advertisement!.IsActive = true;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
