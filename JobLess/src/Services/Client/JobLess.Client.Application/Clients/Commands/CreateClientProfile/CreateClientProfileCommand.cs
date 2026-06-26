using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Enums;
using MediatR;

namespace JobLess.Client.Application.Clients.Commands.CreateClientProfile;

public record CreateClientProfileCommand(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Gender Gender,
    DateTime? DateOfBirth = null,
    string? City = null,
    string? Address = null,
    EducationLevel? EducationLevel = null,
    string? InstitutionName = null,
    int? EducationStartYear = null,
    int? EducationEndYear = null,
    int? YearsOfExperience = null,
    string? Skills = null,
    string? ProfessionalSummary = null,
    string? LinkedInUrl = null,
    bool IsActive = true) : IRequest<ClientProfileDto>;
