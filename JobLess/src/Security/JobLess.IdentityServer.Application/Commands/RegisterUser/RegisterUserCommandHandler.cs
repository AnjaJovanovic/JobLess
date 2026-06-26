using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Application.Interfaces;
using JobLess.IdentityServer.Domain.Enums;
using MediatR;

namespace JobLess.IdentityServer.Application.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthenticationDto>
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtService;

    public RegisterUserCommandHandler(IAuthenticationService authService, IJwtTokenService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
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

