using System.ComponentModel.DataAnnotations;

namespace JobLess.IdentityServer.Application.DTOs;

public class UserCredentialsDto
{
    [Required, EmailAddress]
    public required string Email { get; set; }
    [Required, MinLength(8)]
    public required string Password { get; set; }
}