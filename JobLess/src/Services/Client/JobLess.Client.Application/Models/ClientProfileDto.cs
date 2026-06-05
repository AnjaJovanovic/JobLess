using JobLess.Client.Domain.Enums;

namespace JobLess.Client.Application.Models;

public record ClientProfileDto(
    int ClientId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Gender Gender,
    DateTime CreatedAt,
    bool IsActive);
