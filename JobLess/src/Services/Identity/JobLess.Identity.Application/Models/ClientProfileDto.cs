namespace JobLess.Identity.Application.Models;

public record ClientProfileDto(
    int ClientId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime CreatedAt,
    bool IsActive);
