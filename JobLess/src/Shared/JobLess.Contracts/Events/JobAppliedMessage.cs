namespace JobLess.Contracts.Events;

public record JobAppliedMessage(
    int AdvertisementId,
    int ClientId,
    string ClientEmail,
    string ClientFirstName,
    string ClientLastName,
    string CompanyEmail
);
