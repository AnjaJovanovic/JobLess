using JobLess.Contracts.Events;
using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Application.Interfaces;
using JobLess.IdentityServer.Domain.Enums;
using MediatR;
using MassTransit;

namespace JobLess.IdentityServer.Application.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthenticationDto>
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtService;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterUserCommandHandler(IAuthenticationService authService, IJwtTokenService jwtService, IPublishEndpoint publishEndpoint)
    {
        _authService = authService;
        _jwtService = jwtService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<AuthenticationDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var credentials = new UserCredentialsDto { Email = request.Email, Password = request.Password };
        var role = request.Role.Equals("Company", StringComparison.OrdinalIgnoreCase)
            ? Roles.Company
            : Roles.Candidate;

        var (result, user) = role == Roles.Company
            ? await _authService.RegisterCompanyAsync(credentials)
            : await _authService.RegisterCandidateAsync(credentials);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        var accessToken = _jwtService.GenerateAccessToken(user!);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _publishEndpoint.Publish(
            new UserRegisteredMessage(user!.Id, user.Email!, user.UserRole.ToString()),
            cancellationToken);

        return new AuthenticationDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            UserId = user!.Id,
            Email = user.Email!,
            Role = user.UserRole.ToString()
        };
    }
}

