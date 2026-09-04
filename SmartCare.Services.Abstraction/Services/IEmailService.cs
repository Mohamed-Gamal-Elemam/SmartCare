namespace SmartCare.Services.Abstraction.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}