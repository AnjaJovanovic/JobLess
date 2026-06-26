using JobLess.Notification.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace JobLess.Notification.Infrastructure.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendWelcomeEmailAsync(string toEmail, string role, CancellationToken cancellationToken = default)
    {
        var emailSection = configuration.GetSection("Email");

        var fromAddress = emailSection["FromAddress"] ?? "jobless@gmail.com";
        var fromName = emailSection["FromName"] ?? "JobLess";
        var smtpHost = emailSection["SmtpHost"] ?? "localhost";
        var smtpPort = int.Parse(emailSection["SmtpPort"] ?? "1025");
        var username = emailSection["Username"];
        var password = emailSection["Password"];
        var useSsl = bool.Parse(emailSection["UseSsl"] ?? "false");

        var roleLabel = role.Equals("Company", StringComparison.OrdinalIgnoreCase) ? "kompanija" : "kandidat";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Dobrodošli u JobLess!";

        var builder = new BodyBuilder
        {
            HtmlBody = $"""
                <!DOCTYPE html>
                <html>
                <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;">
                    <h1 style="color: #2563EB;">Dobrodošli u JobLess!</h1>
                    <p>Vaš nalog ({roleLabel}) je uspešno kreiran na platformi <strong>JobLess</strong>.</p>
                    <p>Sada možete:</p>
                    <ul>
                        <li>Pretražujete oglase za posao</li>
                        <li>Aplicirate na pozicije koje vas zanimaju</li>
                        <li>Uredite i dopunite svoj profil</li>
                    </ul>
                    <p style="margin-top: 30px;">Srećno u traženju posla!</p>
                    <p><strong>Tim JobLess</strong></p>
                </body>
                </html>
                """,
            TextBody = $"Dobrodošli u JobLess!\n\nVaš nalog ({roleLabel}) je uspešno kreiran.\n\nSrećno u traženju posla!\nTim JobLess"
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        var socketOptions = useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None;
        await smtp.ConnectAsync(smtpHost, smtpPort, socketOptions, cancellationToken);

        if (!string.IsNullOrEmpty(username))
            await smtp.AuthenticateAsync(username, password, cancellationToken);

        await smtp.SendAsync(message, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Welcome email sent to {Email}", toEmail);
    }
}
