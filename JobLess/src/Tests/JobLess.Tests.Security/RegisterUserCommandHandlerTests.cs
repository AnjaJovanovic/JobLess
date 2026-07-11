using FluentAssertions;
using JobLess.Contracts.Events;
using JobLess.IdentityServer.Application.Commands.RegisterUser;
using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Application.Interfaces;
using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Domain.Enums;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace JobLess.Tests.Security;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthenticationService>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new RegisterUserCommandHandler(
            _authServiceMock.Object, _jwtServiceMock.Object, _publishEndpointMock.Object);
    }

    private static User NoviKorisnik(Roles role) => new()
    {
        Id = Guid.NewGuid(),
        Email = "novi.korisnik@email.rs",
        UserRole = role
    };

    [Fact]
    public async Task Registruje_kandidata_kada_je_rola_Candidate()
    {
        var user = NoviKorisnik(Roles.Candidate);
        _authServiceMock
            .Setup(a => a.RegisterCandidateAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((IdentityResult.Success, (User?)user));
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user)).Returns("access-token");
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

        var result = await _handler.Handle(
            new RegisterUserCommand(user.Email!, "Lozinka123", "Candidate"),
            CancellationToken.None);

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.Role.Should().Be("Candidate");

        _authServiceMock.Verify(a => a.RegisterCompanyAsync(It.IsAny<UserCredentialsDto>()), Times.Never);
    }

    [Fact]
    public async Task Registruje_kompaniju_kada_je_rola_Company()
    {
        var user = NoviKorisnik(Roles.Company);
        _authServiceMock
            .Setup(a => a.RegisterCompanyAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((IdentityResult.Success, user));
        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user)).Returns("access-token");
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

        var result = await _handler.Handle(
            new RegisterUserCommand(user.Email!, "Lozinka123", "Company"),
            CancellationToken.None);

        result.Role.Should().Be("Company");
        _authServiceMock.Verify(a => a.RegisterCandidateAsync(It.IsAny<UserCredentialsDto>()), Times.Never);
    }

    [Theory]
    [InlineData("company")]
    [InlineData("COMPANY")]
    [InlineData("CoMpAnY")]
    public async Task Prepoznaje_rolu_Company_bez_obzira_na_velicinu_slova(string role)
    {
        var user = NoviKorisnik(Roles.Company);
        _authServiceMock
            .Setup(a => a.RegisterCompanyAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((IdentityResult.Success, user));

        await _handler.Handle(
            new RegisterUserCommand(user.Email!, "Lozinka123", role),
            CancellationToken.None);

        _authServiceMock.Verify(a => a.RegisterCompanyAsync(It.IsAny<UserCredentialsDto>()), Times.Once);
    }

    [Theory]
    [InlineData("Candidate")]
    [InlineData("nepoznata-rola")]
    [InlineData("")]
    public async Task Podrazumeva_rolu_Candidate_za_sve_ostalo(string role)
    {
        var user = NoviKorisnik(Roles.Candidate);
        _authServiceMock
            .Setup(a => a.RegisterCandidateAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((IdentityResult.Success, (User?)user));

        await _handler.Handle(
            new RegisterUserCommand(user.Email!, "Lozinka123", role),
            CancellationToken.None);

        _authServiceMock.Verify(a => a.RegisterCandidateAsync(It.IsAny<UserCredentialsDto>()), Times.Once);
        _authServiceMock.Verify(a => a.RegisterCompanyAsync(It.IsAny<UserCredentialsDto>()), Times.Never);
    }

    [Fact]
    public async Task Baca_gresku_kada_registracija_ne_uspe()
    {
        var greska = IdentityResult.Failed(new IdentityError { Description = "Email je već zauzet." });
        _authServiceMock
            .Setup(a => a.RegisterCandidateAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((greska, (User?)null));

        var act = () => _handler.Handle(
            new RegisterUserCommand("zauzet@email.rs", "Lozinka123", "Candidate"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email je već zauzet.");

        _jwtServiceMock.Verify(j => j.GenerateAccessToken(It.IsAny<User>()), Times.Never);
        _publishEndpointMock.Verify(
            p => p.Publish(It.IsAny<UserRegisteredMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Cuva_refresh_token_nakon_uspesne_registracije()
    {
        var user = NoviKorisnik(Roles.Candidate);
        _authServiceMock
            .Setup(a => a.RegisterCandidateAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((IdentityResult.Success, (User?)user));
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("novi-refresh-token");

        await _handler.Handle(
            new RegisterUserCommand(user.Email!, "Lozinka123", "Candidate"),
            CancellationToken.None);

        _authServiceMock.Verify(
            a => a.SaveRefreshTokenAsync(user, "novi-refresh-token", It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task Objavljuje_UserRegisteredMessage_nakon_uspesne_registracije()
    {
        var user = NoviKorisnik(Roles.Candidate);
        _authServiceMock
            .Setup(a => a.RegisterCandidateAsync(It.IsAny<UserCredentialsDto>()))
            .ReturnsAsync((IdentityResult.Success, (User?)user));

        await _handler.Handle(
            new RegisterUserCommand(user.Email!, "Lozinka123", "Candidate"),
            CancellationToken.None);

        _publishEndpointMock.Verify(
            p => p.Publish(
                It.Is<UserRegisteredMessage>(m =>
                    m.UserId == user.Id && m.Email == user.Email && m.Role == "Candidate"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
