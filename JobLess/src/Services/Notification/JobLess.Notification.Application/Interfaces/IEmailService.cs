namespace JobLess.Notification.Application.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string role, CancellationToken cancellationToken = default);
}
