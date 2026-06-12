using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Enums;
using MediatR;

namespace JobLess.Client.Application.Clients.Commands.CreateClientProfile;

public record CreateClientProfileCommand(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Gender Gender,
    bool IsActive = true) : IRequest<ClientProfileDto>;
