namespace JobLess.JobApplication.Application.Interfaces;

public interface IAdvertisementLookupService
{
    Task<AdvertisementLookupResult?> GetByIdAsync(int advertisementId, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<int, string>> GetTitlesByIdsAsync(
        IEnumerable<int> advertisementIds,
        CancellationToken cancellationToken = default);
}

public record AdvertisementLookupResult(
    int AdvertisementId,
    string Title,
    int CompanyId
);
