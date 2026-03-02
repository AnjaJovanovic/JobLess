using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Models;
using JobLess.Advertisement.Application.Queries.GetAll;
using JobLess.Shared.Domain.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Advertisement.Application.Queries.GetOne
{
    public class GetOneAdvertisementQueryHandler : IRequestHandler<GetOneAdvertisementQuery, GetOneAdvertisementResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidationExceptionThrower _validationExceptionThrower;

        public GetOneAdvertisementQueryHandler(
            IApplicationDbContext context,
            IValidationExceptionThrower validationExceptionThrower)
        {
            _context = context;
            _validationExceptionThrower = validationExceptionThrower;
        }

        public async Task<GetOneAdvertisementResult> Handle(
            GetOneAdvertisementQuery query,
            CancellationToken cancellationToken)
        {
            var advertisement = await _context.JobAdvertisements
                .Where(x => x.Id == query.Id && x.IsActive == true)
                .Select(AdvertisementModel.Projection)
                .FirstOrDefaultAsync(cancellationToken);

            if (advertisement == null)
            {
                _validationExceptionThrower
                    .ThrowValidationException("Id", "Advertisement does not exist or is not active.");
            }

            return new GetOneAdvertisementResult
            {
                Advertisement = advertisement
            };
        }
    }
}