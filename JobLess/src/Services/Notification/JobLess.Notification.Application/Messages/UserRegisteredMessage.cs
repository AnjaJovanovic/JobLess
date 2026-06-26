namespace JobLess.Contracts.Events;

public record UserRegisteredMessage(Guid UserId, string Email, string Role);
