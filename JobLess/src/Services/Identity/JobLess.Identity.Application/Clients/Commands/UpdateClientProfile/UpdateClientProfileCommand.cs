using JobLess.Identity.Application.Models;
using MediatR;

namespace JobLess.Identity.Application.Clients.Commands.UpdateClientProfile;

public record UpdateClientProfileCommand(
    int ClientId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive) : IRequest<ClientProfileDto>;
