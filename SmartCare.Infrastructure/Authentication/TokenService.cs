using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartCare.Core.Entities.Identity;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Services.Abstraction.DTOs.Authentication;
using SmartCare.Services.Abstraction.Options;
using SmartCare.Services.Abstraction.Services;

namespace SmartCare.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SmartCareDbContext _context;

    private readonly JwtOptions _jwtOptions;

    public TokenService( UserManager<ApplicationUser> userManager, SmartCareDbContext context, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<string> CreateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new( ClaimTypes.NameIdentifier,user.Id.ToString()),

            new( JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

            new( JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email ?? string.Empty),

            new( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
        };

        claims.AddRange( roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var credentials = new SigningCredentials( key,SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshTokenResult> CreateRefreshTokenAsync( ApplicationUser user)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var tokenHash = HashToken(rawToken);

        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = tokenHash,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        await _context.RefreshTokens.AddAsync(refreshToken);

        return new RefreshTokenResult
        {
            Token = rawToken,
            ExpiresAt = expiresAt
        };
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        return await _context.RefreshTokens.Include(x => x.User).FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task RevokeRefreshTokenAsync( RefreshToken refreshToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData( Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}