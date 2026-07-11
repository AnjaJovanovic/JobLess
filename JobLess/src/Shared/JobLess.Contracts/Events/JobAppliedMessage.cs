namespace JobLess.Contracts.Events;

public record JobAppliedMessage(
    int ApplicationId,
    int AdvertisementId,
    int ClientId,
    string ClientEmail,
    string ClientFirstName,
    string ClientLastName,
    int CompanyId,
    string CompanyEmail
);
