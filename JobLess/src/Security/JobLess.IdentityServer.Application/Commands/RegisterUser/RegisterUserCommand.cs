using JobLess.IdentityServer.Application.DTOs;
using MediatR;
namespace JobLess.IdentityServer.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand (
    string Email,
    string Password,
    string Role
) : IRequest<AuthenticationDto>;
