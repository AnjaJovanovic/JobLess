namespace JobLess.JobApplication.API.Contracts;

public record ApplyForJobRequest(
    int AdvertisementId,
    int CompanyId
);
