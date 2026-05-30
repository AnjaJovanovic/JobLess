using JobLess.Advertisement.Application.Interfaces;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Advertisement.Application.Commands.Delete
{
    public class DeleteAdvertisementCommandHandler
        : IRequestHandler<DeleteAdvertismentCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;

        public DeleteAdvertisementCommandHandler(
            IApplicationDbContext context,
            IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<bool> Handle(
            DeleteAdvertismentCommand command,
            CancellationToken cancellationToken)
        {
            // soft delete -> deactivating advertisement
            var advertisement = await _context.JobAdvertisements
                .Where(x => x.Id == command.Id && x.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (advertisement == null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id",
                        "Advertisement does not exist or has already been deactivated.");
            }

            advertisement!.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}