using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Queries.GetClientProfileByEmail;

public class GetClientProfileByEmailQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetClientProfileByEmailQuery, ClientProfileDto?>
{
    public async Task<ClientProfileDto?> Handle(GetClientProfileByEmailQuery request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email.ToLower() == email, cancellationToken);

        if (client is null)
            return null;

        return new ClientProfileDto(
            client.ClientId,
            client.Email,
            client.FirstName,
            client.LastName,
            client.PhoneNumber,
            client.Gender,
            client.CreatedAt,
            client.IsActive);
    }
}
