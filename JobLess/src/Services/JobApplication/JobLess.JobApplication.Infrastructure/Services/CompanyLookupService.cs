using System.Net.Http.Json;
using System.Text.Json.Serialization;
using JobLess.JobApplication.Application.Interfaces;

namespace JobLess.JobApplication.Infrastructure.Services;

public class CompanyLookupService(HttpClient httpClient) : ICompanyLookupService
{
    public async Task<CompanyLookupResult?> GetByIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/api/Companies/One?id={companyId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<GetCompanyResponse>(cancellationToken);
        var company = payload?.Company;
        if (company is null || string.IsNullOrWhiteSpace(company.Email))
        {
            return null;
        }

        return new CompanyLookupResult(company.Id, company.Email);
    }

    private sealed record GetCompanyResponse(
        [property: JsonPropertyName("company")] CompanyResponse? Company
    );

    private sealed record CompanyResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("email")] string Email
    );
}
