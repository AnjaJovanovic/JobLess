using System.Net.Http.Json;
using System.Text.Json.Serialization;
using JobLess.JobApplication.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JobLess.JobApplication.Infrastructure.Services;

public class AdvertisementLookupService(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor) : IAdvertisementLookupService
{
    public async Task<AdvertisementLookupResult?> GetByIdAsync(
        int advertisementId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/Advertisements/One?id={advertisementId}");

        AddAuthorization(request);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<GetAdvertisementResponse>(cancellationToken);
        var advertisement = payload?.Advertisement;
        if (advertisement is null || string.IsNullOrWhiteSpace(advertisement.Title))
        {
            return null;
        }

        return new AdvertisementLookupResult(advertisement.Id, advertisement.Title.Trim(), advertisement.CompanyId);
    }

    public async Task<IReadOnlyDictionary<int, string>> GetTitlesByIdsAsync(
        IEnumerable<int> advertisementIds,
        CancellationToken cancellationToken = default)
    {
        var uniqueIds = advertisementIds.Where(id => id > 0).Distinct().ToList();
        if (uniqueIds.Count == 0)
        {
            return new Dictionary<int, string>();
        }

        var results = new Dictionary<int, string>();
        foreach (var id in uniqueIds)
        {
            var advertisement = await GetByIdAsync(id, cancellationToken);
            if (advertisement is not null)
            {
                results[id] = advertisement.Title;
            }
        }

        return results;
    }

    private void AddAuthorization(HttpRequestMessage request)
    {
        var authorization = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }
    }

    private sealed record GetAdvertisementResponse(
        [property: JsonPropertyName("advertisement")] AdvertisementResponse? Advertisement
    );

    private sealed record AdvertisementResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("companyId")] int CompanyId
    );
}
