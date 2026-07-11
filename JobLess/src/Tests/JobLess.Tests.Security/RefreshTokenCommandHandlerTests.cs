using FluentAssertions;
using JobLess.IdentityServer.Application.Commands.RefreshToken;
using JobLess.IdentityServer.Application.Interfaces;
using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Domain.Enums;
using Moq;

namespace JobLess.Tests.Security;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthenticationService>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _handler = new RefreshTokenCommandHandler(_authServiceMock.Object, _jwtServiceMock.Object);
    }

    private static User PostojeciKorisnik() => new()
    {
        Id = Guid.NewGuid(),
        Email = "marko.petrovic@email.rs",
        UserRole = Roles.Candidate
    };

    [Fact]
    public async Task Izdaje_novi_par_tokena_za_validan_refresh_token()
    {
        var user = PostojeciKorisnik();
        _authServiceMock
            .Setup(a => a.FindByEmailWithValidRefreshTokenAsync(user.Email!, "stari-refresh-token"))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user)).Returns("novi-access-token");
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("novi-refresh-token");

        var result = await _handler.Handle(
            new RefreshTokenCommand(user.Email!, "stari-refresh-token"),
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("novi-access-token");
        result.RefreshToken.Should().Be("novi-refresh-token");
        result.UserId.Should().Be(user.Id);

        _authServiceMock.Verify(
            a => a.SaveRefreshTokenAsync(user, "novi-refresh-token", It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task Vraca_null_za_nevazeci_ili_istekli_refresh_token()
    {
        _authServiceMock
            .Setup(a => a.FindByEmailWithValidRefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new RefreshTokenCommand("marko.petrovic@email.rs", "istekao-token"),
            CancellationToken.None);

        result.Should().BeNull();
        _jwtServiceMock.Verify(j => j.GenerateAccessToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Rotira_refresh_token_umesto_ponovnog_koriscenja_starog()
    {
        var user = PostojeciKorisnik();
        _authServiceMock
            .Setup(a => a.FindByEmailWithValidRefreshTokenAsync(user.Email!, "stari-refresh-token"))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("novi-refresh-token");

        var result = await _handler.Handle(
            new RefreshTokenCommand(user.Email!, "stari-refresh-token"),
            CancellationToken.None);

        result!.RefreshToken.Should().NotBe("stari-refresh-token");
        result.RefreshToken.Should().Be("novi-refresh-token");
    }
}
