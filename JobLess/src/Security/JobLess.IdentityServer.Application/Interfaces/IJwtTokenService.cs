using JobLess.IdentityServer.Domain.Entities;

namespace JobLess.IdentityServer.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
