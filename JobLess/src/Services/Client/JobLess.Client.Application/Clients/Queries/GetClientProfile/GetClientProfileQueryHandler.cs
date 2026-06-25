using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Mappings;
using JobLess.Client.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Queries.GetClientProfile;

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

        return ClientProfileMapper.ToDto(client);
    }
}
