using JobLess.IdentityServer.Application.DTOs;
using MediatR;

namespace JobLess.IdentityServer.Application.Commands.LoginUser;

public sealed record LoginUserCommand (string Email, string Password) : IRequest<AuthenticationDto?>;