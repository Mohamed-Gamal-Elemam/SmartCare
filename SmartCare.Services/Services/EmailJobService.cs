using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartCare.Core.Entities.Identity;
using SmartCare.Services.Abstraction.Services;

namespace SmartCare.Services.Services;

public class EmailJobService : IEmailJobService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailJobService> _logger;
    private readonly IHostEnvironment _environment;
    private readonly AppBaseUrlOptions _appBaseUrl;

    public EmailJobService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<EmailJobService> logger,
        IOptions<AppBaseUrlOptions> appBaseUrlOptions ,
        IHostEnvironment environment)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
        _environment = environment;
        _appBaseUrl = appBaseUrlOptions.Value;
    }

#if DEBUG
    [AutomaticRetry(
        Attempts = 0,
        OnAttemptsExceeded = AttemptsExceededAction.Delete)]
#else
    [AutomaticRetry(
        Attempts = 3,
        DelaysInSeconds = new[] { 60, 120, 300 },
        OnAttemptsExceeded = AttemptsExceededAction.Delete)]
#endif

    public async Task SendConfirmationEmailJobAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning(
                "Confirmation email job skipped. User {UserId} was not found.",
                userId);

            return;
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation(
                "Confirmation email job skipped. User {UserId} is already confirmed.",
                userId);

            return;
        }

        var token =await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedEmail =Uri.EscapeDataString(user.Email!);

        var encodedToken = Uri.EscapeDataString(token);

        var verificationLink =
            $"{_appBaseUrl.ApiUrl}/api/auth/verify-email" +
            $"?email={encodedEmail}" +
            $"&token={encodedToken}";

        // Development testing
        if (_environment.IsDevelopment())
        {
            _logger.LogInformation(
                "========== VERIFICATION LINK ==========");

            _logger.LogInformation(
                "{VerificationLink}",
                verificationLink);

            _logger.LogInformation(
                "=======================================");
        }

        var body = $"""
            <h2>Welcome to SmartCare</h2>

            <p>Hello {user.FirstName},</p>

            <p>
                Thank you for registering with SmartCare.
                Please verify your email address by clicking the link below:
            </p>

            <p>
                <a href="{verificationLink}">
                    Verify Email
                </a>
            </p>

            <p>
                If you did not create this account, you can safely ignore this email.
            </p>
            """;

        try
        {
            await _emailService.SendEmailAsync( user.Email!, "Verify your SmartCare account", body);

            _logger.LogInformation(
                "Confirmation email sent successfully to {Email}.",
                user.Email);
        }
#if DEBUG
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "SMTP sending failed in Development mode for {Email}. " +
                "The verification link is available in the logs.",
                user.Email);
        }
#else
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send confirmation email to {Email}.",
                user.Email);

            throw;
        }
#endif
    }


    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 120, 300 })]
    public async Task SendPasswordResetEmailJobAsync(Guid userId,string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found.", userId);
            return;
        }

        if (!user.IsActive)
        {
            _logger.LogInformation(
                "User {UserId} is inactive.",
                userId);

            return;
        }

        var encodedToken = Uri.EscapeDataString(token);

        var resetLink =
            $"{_appBaseUrl.ApiUrl}/reset-password" +
            $"?email={Uri.EscapeDataString(user.Email!)}" +
            $"&token={encodedToken}";

        if (_environment.IsDevelopment())
        {
            _logger.LogInformation(
                "Password Reset Link: {ResetLink}",
                resetLink);
        }

        var body = $"""
            <h2>Reset your SmartCare password</h2>
            <p>Hello {user.FirstName},</p>
            <p>We received a request to reset your password.</p>
            <p>
                <a href="{resetLink}">
                    Reset Password
                </a>
            </p>
            <p>
                If you did not request a password reset,
                you can safely ignore this email.
            </p>
            """;

        try
        {
            await _emailService.SendEmailAsync(
                user.Email!,
                "Reset your SmartCare password",
                body);

            _logger.LogInformation(
                "Password reset email sent successfully to {Email}.",
                user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send password reset email to {Email}.",
                user.Email);

            if (!_environment.IsDevelopment())
                throw;
        }
    }
}