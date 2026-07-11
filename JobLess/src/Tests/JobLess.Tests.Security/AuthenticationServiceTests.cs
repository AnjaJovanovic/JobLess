using FluentAssertions;
using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Domain.Enums;
using JobLess.IdentityServer.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace JobLess.Tests.Security;

public class AuthenticationServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _userManagerMock = MockUserManager();
        _signInManagerMock = MockSignInManager(_userManagerMock.Object);
        _service = new AuthenticationService(_userManagerMock.Object, _signInManagerMock.Object);
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static Mock<SignInManager<User>> MockSignInManager(UserManager<User> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        return new Mock<SignInManager<User>>(
            userManager, contextAccessor.Object, claimsFactory.Object, null!, null!, null!, null!);
    }

    private static UserCredentialsDto Kredencijali(
        string email = "marko.petrovic@email.rs", string password = "Lozinka123") =>
        new() { Email = email, Password = password };

    [Fact]
    public async Task RegisterCandidateAsync_postavlja_rolu_Candidate_kada_uspe()
    {
        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), "Lozinka123"))
            .ReturnsAsync(IdentityResult.Success);

        var (result, user) = await _service.RegisterCandidateAsync(Kredencijali());

        result.Succeeded.Should().BeTrue();
        user.Should().NotBeNull();
        user!.UserRole.Should().Be(Roles.Candidate);
        user.UserName.Should().Be("marko.petrovic@email.rs");
        user.Email.Should().Be("marko.petrovic@email.rs");
    }

    [Fact]
    public async Task RegisterCandidateAsync_vraca_null_korisnika_kada_registracija_ne_uspe()
    {
        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Lozinka je preslaba." }));

        var (result, user) = await _service.RegisterCandidateAsync(Kredencijali());

        result.Succeeded.Should().BeFalse();
        user.Should().BeNull();
    }

    [Fact]
    public async Task RegisterCompanyAsync_postavlja_rolu_Company()
    {
        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var (result, user) = await _service.RegisterCompanyAsync(Kredencijali(email: "kompanija@firma.rs"));

        result.Succeeded.Should().BeTrue();
        user.UserRole.Should().Be(Roles.Company);
        user.Email.Should().Be("kompanija@firma.rs");
    }

    [Fact]
    public async Task ValidateUserAsync_vraca_korisnika_za_ispravne_kredencijale()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "marko.petrovic@email.rs", UserRole = Roles.Candidate };
        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _signInManagerMock
            .Setup(s => s.CheckPasswordSignInAsync(user, "Lozinka123", false))
            .ReturnsAsync(SignInResult.Success);

        var result = await _service.ValidateUserAsync(Kredencijali(password: "Lozinka123"));

        result.Should().Be(user);
    }

    [Fact]
    public async Task ValidateUserAsync_vraca_null_kada_korisnik_ne_postoji()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _service.ValidateUserAsync(Kredencijali());

        result.Should().BeNull();
        _signInManagerMock.Verify(
            s => s.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidateUserAsync_vraca_null_kada_je_lozinka_pogresna()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "marko.petrovic@email.rs", UserRole = Roles.Candidate };
        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _signInManagerMock
            .Setup(s => s.CheckPasswordSignInAsync(user, It.IsAny<string>(), false))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _service.ValidateUserAsync(Kredencijali(password: "PogresnaLozinka"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveRefreshTokenAsync_upisuje_token_i_datum_isteka()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "marko.petrovic@email.rs" };
        var expiry = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _userManagerMock
            .Setup(m => m.SetAuthenticationTokenAsync(user, "JobLess", It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        await _service.SaveRefreshTokenAsync(user, "moj-refresh-token", expiry);

        _userManagerMock.Verify(
            m => m.SetAuthenticationTokenAsync(user, "JobLess", "RefreshToken", "moj-refresh-token"),
            Times.Once);
        _userManagerMock.Verify(
            m => m.SetAuthenticationTokenAsync(user, "JobLess", "RefreshTokenExpiry", expiry.ToString("O")),
            Times.Once);
    }

    [Fact]
    public async Task FindByEmailWithValidRefreshTokenAsync_vraca_korisnika_za_vazeci_token()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "marko.petrovic@email.rs" };
        var expiry = DateTime.UtcNow.AddMinutes(5);
        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.GetAuthenticationTokenAsync(user, "JobLess", "RefreshToken"))
            .ReturnsAsync("vazeci-token");
        _userManagerMock
            .Setup(m => m.GetAuthenticationTokenAsync(user, "JobLess", "RefreshTokenExpiry"))
            .ReturnsAsync(expiry.ToString("O"));

        var result = await _service.FindByEmailWithValidRefreshTokenAsync(user.Email!, "vazeci-token");

        result.Should().Be(user);
    }

    [Fact]
    public async Task FindByEmailWithValidRefreshTokenAsync_vraca_null_kada_se_token_ne_poklapa()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "marko.petrovic@email.rs" };
        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.GetAuthenticationTokenAsync(user, "JobLess", "RefreshToken"))
            .ReturnsAsync("sacuvani-token");
        _userManagerMock
            .Setup(m => m.GetAuthenticationTokenAsync(user, "JobLess", "RefreshTokenExpiry"))
            .ReturnsAsync(DateTime.UtcNow.AddMinutes(5).ToString("O"));

        var result = await _service.FindByEmailWithValidRefreshTokenAsync(user.Email!, "poslati-drugi-token");

        result.Should().BeNull();
    }

    [Fact]
    public async Task FindByEmailWithValidRefreshTokenAsync_vraca_null_kada_je_token_istekao()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "marko.petrovic@email.rs" };
        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.GetAuthenticationTokenAsync(user, "JobLess", "RefreshToken"))
            .ReturnsAsync("istekli-token");
        _userManagerMock
            .Setup(m => m.GetAuthenticationTokenAsync(user, "JobLess", "RefreshTokenExpiry"))
            .ReturnsAsync(DateTime.UtcNow.AddMinutes(-5).ToString("O"));

        var result = await _service.FindByEmailWithValidRefreshTokenAsync(user.Email!, "istekli-token");

        result.Should().BeNull();
    }

    [Fact]
    public async Task FindByEmailWithValidRefreshTokenAsync_vraca_null_kada_korisnik_ne_postoji()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _service.FindByEmailWithValidRefreshTokenAsync("nepostojeci@email.rs", "bilo-koji-token");

        result.Should().BeNull();
        _userManagerMock.Verify(
            m => m.GetAuthenticationTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}
