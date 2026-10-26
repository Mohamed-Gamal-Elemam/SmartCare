using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SmartCare.Core.Entities.Identity;
using SmartCare.Core.Entities.Patients;
using SmartCare.Core.Interfaces;
using SmartCare.Services.Abstraction.DTOs.Authentication;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Constants;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IOptions<AppBaseUrlOptions> _appBaseUrl;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IBackgroundJobClient backgroundJobClient,
        IOptions<AppBaseUrlOptions> appBaseUrl)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _backgroundJobClient = backgroundJobClient;
        _appBaseUrl = appBaseUrl;
    }

    public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser != null)
            throw new ConflictException("Email is already registered.");

        if (!await _roleManager.RoleExistsAsync(RoleConstants.Patient))
            throw new NotFoundException("Patient role does not exist.");

        // ============ 1) Transaction تغطي إنشاء الـ User + Patient ============
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = normalizedEmail,
                UserName = normalizedEmail,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.Patient);
            if (!roleResult.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));

            // ============ 2) الأهم: إنشاء Patient entity ============
            var patientRepository = _unitOfWork.GetRepository<Patient, Guid>();

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                // ⬇️ املأ الحقول دي حسب الـ Patient entity عندك
                // DateOfBirth = dto.DateOfBirth,
                // Gender = dto.Gender,
                // PhoneNumber = dto.PhoneNumber,
                // Address = dto.Address,
            };

            await patientRepository.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            // ============ 3) Email verification job (بعد الـ commit) ============
            _backgroundJobClient.Enqueue<IEmailJobService>(
                x => x.SendConfirmationEmailJobAsync(user.Id));

            var response = new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                RequiresEmailVerification = true
            };

            return ApiResponse<RegisterResponseDto>.Ok(
                response,
                "Registration successful. Please verify your email.");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }


    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(
            normalizedEmail);

        if (user == null || !await _userManager.CheckPasswordAsync(user,dto.Password))
            throw new UnauthorizedException("Invalid email or password.");
       

        if (!user.IsActive)
            throw new ForbiddenException("Your account is inactive.");

        if (!user.EmailConfirmed)
            throw new UnauthorizedException("Please verify your email before logging in.");

        var authResponse =await GenerateAuthenticationResponseAsync(user);

        return ApiResponse<AuthResponseDto>.Ok( authResponse,"Login successful.");
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync( RefreshTokenDto dto)
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync(dto.RefreshToken);

        if (refreshToken == null || !refreshToken.IsActive)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var user = refreshToken.User;

        if (!user.IsActive)
            throw new ForbiddenException("Your account is inactive.");

        refreshToken.RevokedAt = DateTime.UtcNow;

        var accessToken = await _tokenService.CreateAccessTokenAsync(user);

        var newRefreshToken =await _tokenService.CreateRefreshTokenAsync(user);

        await _unitOfWork.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);

        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            }
        };

        return ApiResponse<AuthResponseDto>.Ok( response, "Token refreshed successfully.");
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            throw new BadRequestException("Invalid user ID.");

        var refreshTokens = await _unitOfWork.GetRepository<RefreshToken, Guid>().GetAllTrackedAsync(rt => rt.UserId == userGuid);

        foreach (var token in refreshTokens)
            token.RevokedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok( true, "Logged out successfully.");
    }

    public async Task<ApiResponse<bool>> VerifyEmailAsync(  VerifyEmailDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(normalizedEmail);

        if (user == null)
            throw new BadRequestException("Invalid verification request.");

        if (user.EmailConfirmed)
        {
            return ApiResponse<bool>.Ok( true,"Email is already verified.");
        }

        var result = await _userManager.ConfirmEmailAsync( user, dto.Token);

        if (!result.Succeeded)
        {
            throw new BadRequestException( "Invalid or expired verification token.");
        }

        return ApiResponse<bool>.Ok( true,  "Email verified successfully.");
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync( normalizedEmail);

        if (user == null || !user.IsActive)
        {
            return ApiResponse<bool>.Ok( true,"If the email exists, a password reset link has been sent.");
        }

        var token =
            await _userManager.GeneratePasswordResetTokenAsync(user);

        _backgroundJobClient.Enqueue<IEmailJobService>(x => x.SendPasswordResetEmailJobAsync( user.Id,token));

        return ApiResponse<bool>.Ok( true,"If the email exists, a password reset link has been sent.");
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync( ResetPasswordDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(normalizedEmail);

        if (user == null)
            throw new BadRequestException("Invalid password reset request.");

        var result = await _userManager.ResetPasswordAsync(user,dto.Token,dto.NewPassword);

        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ",result.Errors.Select(e => e.Description)));
        }

        return ApiResponse<bool>.Ok(true,"Password reset successfully.");
    }

    private async Task<AuthResponseDto> GenerateAuthenticationResponseAsync( ApplicationUser user)
    {
        var accessToken = await _tokenService.CreateAccessTokenAsync(user);

        var refreshToken =  await _tokenService.CreateRefreshTokenAsync(user);

        await _unitOfWork.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            }
        };
    }
}