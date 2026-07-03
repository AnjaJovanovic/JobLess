namespace JobLess.Contracts.Events;

public record ApplicationStatusChangedMessage(
    int ApplicationId,
    int AdvertisementId,
    string CandidateEmail,
    string CandidateFirstName,
    string CandidateLastName,
    string NewStatus
);
