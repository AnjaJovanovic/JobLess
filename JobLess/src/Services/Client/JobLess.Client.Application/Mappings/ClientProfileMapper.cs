using ClientEntity = JobLess.Client.Domain.Entities.Client;
using JobLess.Client.Application.Models;

namespace JobLess.Client.Application.Mappings;

public static class ClientProfileMapper
{
    public static ClientProfileDto ToDto(ClientEntity client) =>
        new(
            client.ClientId,
            client.Email,
            client.FirstName,
            client.LastName,
            client.PhoneNumber,
            client.Gender,
            client.DateOfBirth,
            client.City,
            client.Address,
            client.EducationLevel,
            client.InstitutionName,
            client.EducationStartYear,
            client.EducationEndYear,
            client.YearsOfExperience,
            client.Skills,
            client.ProfessionalSummary,
            client.LinkedInUrl,
            client.CreatedAt,
            client.IsActive);

    public static void ApplyProfileFields(
        ClientEntity client,
        DateTime? dateOfBirth,
        string? city,
        string? address,
        Domain.Enums.EducationLevel? educationLevel,
        string? institutionName,
        int? educationStartYear,
        int? educationEndYear,
        int? yearsOfExperience,
        string? skills,
        string? professionalSummary,
        string? linkedInUrl)
    {
        client.DateOfBirth = dateOfBirth;
        client.City = city;
        client.Address = address;
        client.EducationLevel = educationLevel;
        client.InstitutionName = institutionName;
        client.EducationStartYear = educationStartYear;
        client.EducationEndYear = educationEndYear;
        client.YearsOfExperience = yearsOfExperience;
        client.Skills = skills;
        client.ProfessionalSummary = professionalSummary;
        client.LinkedInUrl = linkedInUrl;
    }
}
