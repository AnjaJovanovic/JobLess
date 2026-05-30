using JobLess.Identity.Application.Interfaces;
using JobLess.Identity.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Identity.Application.Clients.Queries.GetClientProfile;

public class GetClientProfileQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetClientProfileQuery, ClientProfileDto?>
{
    public async Task<ClientProfileDto?> Handle(GetClientProfileQuery request, CancellationToken cancellationToken)
    {
        var client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClientId == request.ClientId, cancellationToken);

        if (client is null)
            return null;

        return new ClientProfileDto(
            client.ClientId,
            client.FirstName,
            client.LastName,
            client.PhoneNumber,
            client.CreatedAt,
            client.IsActive);
    }
}
