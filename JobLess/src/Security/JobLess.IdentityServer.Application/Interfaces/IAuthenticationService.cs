using JobLess.IdentityServer.Domain.Entities;
using JobLess.IdentityServer.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace JobLess.IdentityServer.Application.Interfaces;

public interface IAuthenticationService
{
    Task<(IdentityResult, User? User)> RegisterCandidateAsync(UserCredentialsDto dto); 
    Task<(IdentityResult, User User)> RegisterCompanyAsync (UserCredentialsDto dto);
    Task<User?> ValidateUserAsync (UserCredentialsDto dto); 
    Task SaveRefreshTokenAsync(User user, string refreshToken, DateTime expiry);
    Task<User?> FindByEmailWithValidRefreshTokenAsync(string email, string refreshToken);
}