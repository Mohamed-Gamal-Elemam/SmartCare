using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Services.Abstraction.DTOs.Authentication;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Responses;

namespace SmartCare.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register(RegisterDto dto)
    {
        return Ok(await _authService.RegisterAsync(dto));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto dto)
    {
        return Ok(await _authService.LoginAsync(dto));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken(RefreshTokenDto dto)
    {
        return Ok(await _authService.RefreshTokenAsync(dto));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("User identity is missing.");

        return Ok(await _authService.LogoutAsync(userId));
    }

    [HttpGet("verify-email")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> VerifyEmail( [FromQuery] VerifyEmailDto dto)
    {
        return Ok(await _authService.VerifyEmailAsync(dto));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword(ForgotPasswordDto dto)
    {
        return Ok(await _authService.ForgotPasswordAsync(dto));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(ResetPasswordDto dto)
    {
        return Ok(await _authService.ResetPasswordAsync(dto));
    }
}