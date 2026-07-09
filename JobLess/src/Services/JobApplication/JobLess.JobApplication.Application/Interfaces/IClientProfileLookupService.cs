namespace JobLess.JobApplication.Application.Interfaces;

public interface IClientProfileLookupService
{
    Task<ClientProfileLookupResult?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public record ClientProfileLookupResult(
    int ClientId,
    string FirstName,
    string LastName,
    string Email
);
