using JobLess.Identity.Application.Models;
using MediatR;

namespace JobLess.Identity.Application.Clients.Commands.CreateClientProfile;

public record CreateClientProfileCommand(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive = true) : IRequest<ClientProfileDto>;
