namespace JobLess.Contracts.Events;

public record ApplicationStatusChangedMessage(
    int ApplicationId,
    int AdvertisementId,
    int CompanyId,
    string CandidateEmail,
    string CandidateFirstName,
    string CandidateLastName,
    string NewStatus
);
