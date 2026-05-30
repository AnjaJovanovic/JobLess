using JobLess.Identity.Application.Interfaces;
using JobLess.Identity.Application.Models;
using JobLess.Identity.Domain.Entities;
using MediatR;

namespace JobLess.Identity.Application.Clients.Commands.CreateClientProfile;

public class CreateClientProfileCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateClientProfileCommand, ClientProfileDto>
{
    public async Task<ClientProfileDto> Handle(CreateClientProfileCommand request, CancellationToken cancellationToken)
    {
        var client = new Client
        {
            Email = string.Empty,
            PasswordHash = string.Empty,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync(cancellationToken);

        return ToDto(client);
    }

    private static ClientProfileDto ToDto(Client client) =>
        new(
            client.ClientId,
            client.FirstName,
            client.LastName,
            client.PhoneNumber,
            client.CreatedAt,
            client.IsActive);
}
