using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Enums;
using MediatR;

namespace JobLess.Client.Application.Clients.Commands.UpdateClientProfile;

public record UpdateClientProfileCommand(
    int ClientId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Gender Gender,
    bool IsActive) : IRequest<ClientProfileDto>;
