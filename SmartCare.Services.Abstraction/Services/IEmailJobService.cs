
namespace SmartCare.Services.Abstraction.Services;

public interface IEmailJobService
{
    Task SendConfirmationEmailJobAsync(Guid userId);
    Task SendPasswordResetEmailJobAsync(Guid userId, string token);
}