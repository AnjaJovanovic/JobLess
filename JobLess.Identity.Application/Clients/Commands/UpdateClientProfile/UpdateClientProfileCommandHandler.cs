using JobLess.Identity.Application.Interfaces;
using JobLess.Identity.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Identity.Application.Clients.Commands.UpdateClientProfile;

public class UpdateClientProfileCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateClientProfileCommand, ClientProfileDto>
{
    public async Task<ClientProfileDto> Handle(UpdateClientProfileCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == request.ClientId, cancellationToken);

        if (client is null)
            throw new KeyNotFoundException($"Klijent sa ID {request.ClientId} nije pronađen.");

        client.FirstName = request.FirstName;
        client.LastName = request.LastName;
        client.PhoneNumber = request.PhoneNumber;
        client.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return new ClientProfileDto(
            client.ClientId,
            client.FirstName,
            client.LastName,
            client.PhoneNumber,
            client.CreatedAt,
            client.IsActive);
    }
}
