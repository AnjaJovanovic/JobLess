using JobLess.Client.Domain.Enums;

namespace JobLess.Client.Domain.Entities;

public class Client
{
    public int ClientId { get; set; }

    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    public string? PhoneNumber { get; set; }

    public Gender Gender { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}