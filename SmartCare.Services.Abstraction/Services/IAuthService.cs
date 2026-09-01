using SmartCare.Services.Abstraction.DTOs.Authentication;

namespace SmartCare.Services.Abstraction.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    Task<AuthResponseDto> LoginAsync(LoginDto dto);

    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);

    Task LogoutAsync(RefreshTokenDto dto);

    Task VerifyEmailAsync(VerifyEmailDto dto);

    Task ForgotPasswordAsync(ForgotPasswordDto dto);

    Task ResetPasswordAsync(ResetPasswordDto dto);

    Task ChangePasswordAsync( Guid userId, ChangePasswordDto dto);
}