using JobLess.IdentityServer.Application.DTOs;
using JobLess.IdentityServer.Application.Interfaces;
using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace JobLess.IdentityServer.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(IdentityResult, User?)> RegisterCandidateAsync(UserCredentialsDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            UserRole = Roles.Candidate
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        return (result, result.Succeeded ? user : null);
    }

    public async Task<(IdentityResult, User)> RegisterCompanyAsync(UserCredentialsDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            UserRole = Roles.Company
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        return (result, user);
    }

    public async Task<User?> ValidateUserAsync(UserCredentialsDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return null;

        var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        return check.Succeeded ? user : null;
    }
    public async Task SaveRefreshTokenAsync(User user, string refreshToken, DateTime expiry)
    {
        await _userManager.SetAuthenticationTokenAsync(user, "JobLess", "RefreshToken", refreshToken);
        await _userManager.SetAuthenticationTokenAsync(user, "JobLess", "RefreshTokenExpiry", expiry.ToString("O"));
    }

    public async Task<User?> FindByEmailWithValidRefreshTokenAsync(string email, string refreshToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "JobLess", "RefreshToken");
        var storedExpiry = await _userManager.GetAuthenticationTokenAsync(user, "JobLess", "RefreshTokenExpiry");

        if (storedToken != refreshToken) return null;
        if (!DateTime.TryParse(storedExpiry, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiry)
            || expiry < DateTime.UtcNow)
            return null;

        return user;
    }
}
