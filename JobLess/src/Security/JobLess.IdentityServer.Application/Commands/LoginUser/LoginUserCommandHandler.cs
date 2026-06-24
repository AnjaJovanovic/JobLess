using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Application.Interfaces;
using MediatR;

namespace JobLess.IdentityServer.Application.Commands.LoginUser;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthenticationDto?>
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtService;

    public LoginUserCommandHandler(IAuthenticationService authService, IJwtTokenService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    public async Task<AuthenticationDto?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var credentials = new UserCredentialsDto { Email = request.Email, Password = request.Password };
        var user = await _authService.ValidateUserAsync(credentials);

        if (user is null) return null;

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return new AuthenticationDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            UserId = user.Id,
            Email = user.Email!,
            Role = user.UserRole.ToString()
        };
    }
}
