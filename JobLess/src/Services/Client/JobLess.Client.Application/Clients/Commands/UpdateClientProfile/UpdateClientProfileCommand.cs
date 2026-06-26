using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Enums;
using MediatR;

namespace JobLess.Client.Application.Clients.Commands.UpdateClientProfile;

public record UpdateClientProfileCommand(
    int ClientId,
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
    bool IsActive) : IRequest<ClientProfileDto>;
