using System.Security.Claims;
using LogisticsPlatform.Domain.Entities;

namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);

    string GenerateRefreshToken(Guid userId);

    bool TryValidateRefreshToken(string refreshToken, out ClaimsPrincipal? principal);

    string HashRefreshToken(string token);

    /// <summary>Legacy alias for GenerateAccessToken.</summary>
    string GenerateToken(ApplicationUser user, IList<string> roles) =>
        GenerateAccessToken(user, roles);
}
