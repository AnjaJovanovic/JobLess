namespace JobLess.JobApplication.Application.Interfaces;

public interface ICompanyLookupService
{
    Task<CompanyLookupResult?> GetByIdAsync(int companyId, CancellationToken cancellationToken = default);
}

public record CompanyLookupResult(
    int CompanyId,
    string Email
);
