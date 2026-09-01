using SmartCare.Core.Entities.Identity;
using SmartCare.Services.Abstraction.DTOs.Authentication;

namespace SmartCare.Services.Abstraction.Services;

public interface ITokenService
{
    Task<string> CreateAccessTokenAsync(ApplicationUser user);

    Task<RefreshTokenResult> CreateRefreshTokenAsync(ApplicationUser user);

    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);

    Task RevokeRefreshTokenAsync( RefreshToken refreshToken);
}