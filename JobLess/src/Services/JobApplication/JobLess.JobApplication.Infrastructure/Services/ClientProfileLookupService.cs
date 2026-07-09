using System.Net.Http.Json;
using System.Text.Json.Serialization;
using JobLess.JobApplication.Application.Interfaces;

namespace JobLess.JobApplication.Infrastructure.Services;

public class ClientProfileLookupService(HttpClient httpClient) : IClientProfileLookupService
{
    public async Task<ClientProfileLookupResult?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var encodedEmail = Uri.EscapeDataString(email);
        var response = await httpClient.GetAsync($"/api/clients/profile/by-email?email={encodedEmail}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var profile = await response.Content.ReadFromJsonAsync<ClientProfileResponse>(cancellationToken);
        if (profile is null)
        {
            return null;
        }

        return new ClientProfileLookupResult(
            profile.ClientId,
            profile.FirstName,
            profile.LastName,
            profile.Email);
    }

    private sealed record ClientProfileResponse(
        [property: JsonPropertyName("clientId")] int ClientId,
        [property: JsonPropertyName("firstName")] string FirstName,
        [property: JsonPropertyName("lastName")] string LastName,
        [property: JsonPropertyName("email")] string Email
    );
}
