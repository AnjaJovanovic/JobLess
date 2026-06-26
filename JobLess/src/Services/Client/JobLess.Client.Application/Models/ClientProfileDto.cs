using JobLess.Client.Domain.Enums;

namespace JobLess.Client.Application.Models;

public record ClientProfileDto(
    int ClientId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Gender Gender,
    DateTime? DateOfBirth,
    string? City,
    string? Address,
    EducationLevel? EducationLevel,
    string? InstitutionName,
    int? EducationStartYear,
    int? EducationEndYear,
    int? YearsOfExperience,
    string? Skills,
    string? ProfessionalSummary,
    string? LinkedInUrl,
    DateTime CreatedAt,
    bool IsActive);
