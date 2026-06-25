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

    public DateTime? DateOfBirth { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public EducationLevel? EducationLevel { get; set; }

    public string? InstitutionName { get; set; }

    public int? EducationStartYear { get; set; }

    public int? EducationEndYear { get; set; }

    public int? YearsOfExperience { get; set; }

    public string? Skills { get; set; }

    public string? ProfessionalSummary { get; set; }

    public string? LinkedInUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
