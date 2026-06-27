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
        var emailSection = configuration.GetSection("Smtp");

        var fromAddress = emailSection["FromEmail"] ?? "jobless.matf@gmail.com";
        var fromName = emailSection["FromName"] ?? "JobLess";
        var smtpHost = emailSection["Host"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(emailSection["Port"] ?? "587");
        var username = emailSection["UserName"];
        var password = emailSection["Password"];

        var roleLabel = role.Equals("Company", StringComparison.OrdinalIgnoreCase) ? "kompanija" : "kandidat";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Dobrodošli u JobLess!";

        bool isCompany = role.Equals("Company", StringComparison.OrdinalIgnoreCase);
        string feature1 = "Pretražujete oglase za posao";
        string feature2 = "Aplicirate na pozicije koje vas zanimaju";
        string feature3 = "Uredite i dopunite svoj profil";

        if (isCompany)
        {
            feature1 = "Objavljujete nove oglase za posao";
            feature2 = "Pregledate prijavljene kandidate";
            feature3 = "Uredite i dopunite profil svoje kompanije";
        }

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
                        <li>{feature1}</li>
                        <li>{feature2}</li>
                        <li>{feature3}</li>
                    </ul>
                    <p style="margin-top: 30px;">Srećno!</p>
                    <p><strong>Tim JobLess</strong></p>
                </body>
                </html>
                """,
            TextBody = $"Dobrodošli u JobLess!\n\nVaš nalog ({roleLabel}) je uspešno kreiran.\n\nSrećno u traženju posla!\nTim JobLess"
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);

        if (!string.IsNullOrEmpty(username))
            await smtp.AuthenticateAsync(username, password, cancellationToken);

        await smtp.SendAsync(message, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Welcome email sent to {Email}", toEmail);
    }
}
