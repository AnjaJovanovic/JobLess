using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Domain.Enums;
using JobLess.IdentityServer.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace JobLess.Tests.Security;

public class JwtTokenServiceTests
{
    private static readonly User TestUser = new()
    {
        Id = Guid.NewGuid(),
        Email = "marko.petrovic@email.rs",
        UserRole = Roles.Company
    };

    private static IConfiguration BuildConfiguration(string? expirationMinutes = "5") =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "JobLess-Test-Signing-Key-Minimum-32-Chars!",
                ["Jwt:Issuer"] = "JobLess.Auth.Test",
                ["Jwt:Audience"] = "JobLess.Client.Test",
                ["Jwt:ExpirationMinutes"] = expirationMinutes,
            })
            .Build();

    [Fact]
    public void Generise_access_token_sa_ispravnim_claim_ovima()
    {
        var service = new JwtTokenService(BuildConfiguration());

        var token = service.GenerateAccessToken(TestUser);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Issuer.Should().Be("JobLess.Auth.Test");
        jwt.Audiences.Should().Contain("JobLess.Client.Test");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == TestUser.Email);
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == TestUser.Id.ToString());
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == TestUser.Email);
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Company");
    }

    [Fact]
    public void Postavlja_rok_vazenja_prema_konfiguraciji()
    {
        var service = new JwtTokenService(BuildConfiguration(expirationMinutes: "5"));

        var token = service.GenerateAccessToken(TestUser);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(5), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Koristi_podrazumevani_rok_od_60_minuta_kada_konfiguracija_nedostaje()
    {
        var service = new JwtTokenService(BuildConfiguration(expirationMinutes: null));

        var token = service.GenerateAccessToken(TestUser);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(60), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Svaki_access_token_ima_jedinstven_jti()
    {
        var service = new JwtTokenService(BuildConfiguration());

        var jwt1 = new JwtSecurityTokenHandler().ReadJwtToken(service.GenerateAccessToken(TestUser));
        var jwt2 = new JwtSecurityTokenHandler().ReadJwtToken(service.GenerateAccessToken(TestUser));

        var jti1 = jwt1.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var jti2 = jwt2.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        jti1.Should().NotBe(jti2);
    }

    [Fact]
    public void Generise_razlicit_refresh_token_svaki_put()
    {
        var service = new JwtTokenService(BuildConfiguration());

        var token1 = service.GenerateRefreshToken();
        var token2 = service.GenerateRefreshToken();

        token1.Should().NotBe(token2);
    }

    [Fact]
    public void Refresh_token_ima_dovoljnu_entropiju()
    {
        var service = new JwtTokenService(BuildConfiguration());

        var token = service.GenerateRefreshToken();
        var bytes = Convert.FromBase64String(token);

        bytes.Should().HaveCount(64);
    }
}
