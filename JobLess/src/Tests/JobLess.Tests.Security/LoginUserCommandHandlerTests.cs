using FluentAssertions;
using JobLess.IdentityServer.Application.Commands.LoginUser;
using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Application.Interfaces;
using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Domain.Enums;
using Moq;

namespace JobLess.Tests.Security;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthenticationService>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _handler = new LoginUserCommandHandler(_authServiceMock.Object, _jwtServiceMock.Object);
    }

    private static User PostojeciKandidat() => new()
    {
        Id = Guid.NewGuid(),
        Email = "marko.petrovic@email.rs",
        UserRole = Roles.Candidate
    };

    [Fact]
    public async Task Prijavljuje_korisnika_sa_ispravnim_kredencijalima()
    {
        var user = PostojeciKandidat();
        _authServiceMock.Setup(a => a.ValidateUserAsync(It.Is<UserCredentialsDto>(
                d => d.Email == user.Email && d.Password == "Lozinka123")))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user)).Returns("access-token");
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

        var result = await _handler.Handle(
            new LoginUserCommand(user.Email!, "Lozinka123"),
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.UserId.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Role.Should().Be("Candidate");

        _authServiceMock.Verify(
            a => a.SaveRefreshTokenAsync(user, "refresh-token", It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task Vraca_null_kada_su_kredencijali_pogresni()
    {
        _authServiceMock.Setup(a => a.ValidateUserAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new LoginUserCommand("nepostojeci@email.rs", "PogresnaLozinka1"),
            CancellationToken.None);

        result.Should().BeNull();
        _jwtServiceMock.Verify(j => j.GenerateAccessToken(It.IsAny<User>()), Times.Never);
        _authServiceMock.Verify(
            a => a.SaveRefreshTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task Prosledjuje_ispravne_kredencijale_servisu_za_validaciju()
    {
        UserCredentialsDto? poslati = null;
        _authServiceMock.Setup(a => a.ValidateUserAsync(It.IsAny<UserCredentialsDto>()))
            .Callback<UserCredentialsDto>(dto => poslati = dto)
            .ReturnsAsync((User?)null);

        await _handler.Handle(
            new LoginUserCommand("korisnik@email.rs", "TajnaLozinka1"),
            CancellationToken.None);

        poslati.Should().NotBeNull();
        poslati!.Email.Should().Be("korisnik@email.rs");
        poslati.Password.Should().Be("TajnaLozinka1");
    }
}
