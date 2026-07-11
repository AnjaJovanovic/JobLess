using System.Net.Http.Json;
using System.Text.Json.Serialization;
using JobLess.JobApplication.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JobLess.JobApplication.Infrastructure.Services;

public class ClientProfileLookupService(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor) : IClientProfileLookupService
{
    public async Task<ClientProfileLookupResult?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var encodedEmail = Uri.EscapeDataString(email);
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/clients/profile/by-email?email={encodedEmail}");

        var authorization = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);

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
