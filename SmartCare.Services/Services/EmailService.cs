using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartCare.Services.Abstraction.Options;
using SmartCare.Services.Abstraction.Services;

namespace SmartCare.Services.Services;

public class EmailService : IEmailService
{
    private readonly MailSettingsOptions _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<MailSettingsOptions> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body)
    {
        try
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _emailSettings.DisplayName,
                    _emailSettings.From));

            email.To.Add(
                MailboxAddress.Parse(to));

            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            email.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                _emailSettings.SmtpHost,
                _emailSettings.SmtpPort,
                SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(
                _emailSettings.Username,
                _emailSettings.Password);

            await smtpClient.SendAsync(email);

            await smtpClient.DisconnectAsync(true);

            _logger.LogInformation(
                "Email sent successfully to {Email}.",
                to);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {Email}.",
                to);

            throw;
        }
    }
}