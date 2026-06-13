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
}
