using JobLess.IdentityServer.Application.DTOs;
using MediatR;

namespace JobLess.IdentityServer.Application.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Email, string RefreshToken) : IRequest<AuthenticationDto?>;
