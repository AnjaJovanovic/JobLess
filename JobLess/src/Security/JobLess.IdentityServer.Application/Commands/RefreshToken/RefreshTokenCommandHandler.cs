using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Application.Interfaces;
using MediatR;

namespace JobLess.IdentityServer.Application.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationDto?>
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtService;

    public RefreshTokenCommandHandler(IAuthenticationService authService, IJwtTokenService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    public async Task<AuthenticationDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.FindByEmailWithValidRefreshTokenAsync(request.Email, request.RefreshToken);
        if (user is null) return null;

        var accessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddMinutes(10);

        await _authService.SaveRefreshTokenAsync(user, newRefreshToken, refreshExpiry);

        return new AuthenticationDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            UserId = user.Id,
            Email = user.Email!,
            Role = user.UserRole.ToString()
        };
    }
}
