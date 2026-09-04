using SmartCare.Services.Abstraction.DTOs.Authentication;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Abstraction.Services;

public interface IAuthService
{
    
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync( RegisterDto dto);

        Task<ApiResponse<AuthResponseDto>> LoginAsync( LoginDto dto);

        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);

        Task<ApiResponse<bool>> LogoutAsync( string userId);

        Task<ApiResponse<bool>> VerifyEmailAsync(VerifyEmailDto dto);

        Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto);

        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto);
    
}