using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartCare.Core.Entities.Doctors;
using SmartCare.Core.Entities.Identity;
using SmartCare.Core.Entities.Patients;
using SmartCare.Core.Interfaces;
using SmartCare.Services.Abstraction.DTOs.Authentication;
using SmartCare.Services.Abstraction.Options;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartCare.Services.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtOptions _jwtOptions;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<string> CreateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        if (roles.Contains(RoleConstants.Doctor))
        {
            var doctors = await _unitOfWork.GetRepository<Doctor, Guid>()
                .GetAllAsync(d => d.UserId == user.Id);

            var doctorProfile = doctors.FirstOrDefault();
            if (doctorProfile is not null)
                claims.Add(new Claim("DoctorId", doctorProfile.Id.ToString()));
        }

        if (roles.Contains(RoleConstants.Patient))
        {
            var patients = await _unitOfWork.GetRepository<Patient, Guid>()
                .GetAllAsync(p => p.UserId == user.Id);

            var patientProfile = patients.FirstOrDefault();
            if (patientProfile is not null)
                claims.Add(new Claim("PatientId", patientProfile.Id.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshTokenResult> CreateRefreshTokenAsync(ApplicationUser user)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenHash = HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = tokenHash,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays)
        };

        await _unitOfWork.GetRepository<RefreshToken, Guid>().AddAsync(refreshToken);

        return new RefreshTokenResult
        {
            Token = rawToken,
            ExpiresAt = refreshToken.ExpiresAt
        };
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);
        var tokens = await _unitOfWork.GetRepository<RefreshToken, Guid>()
            .GetAllAsync(rt => rt.TokenHash == tokenHash);

        return tokens.FirstOrDefault();
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }
}