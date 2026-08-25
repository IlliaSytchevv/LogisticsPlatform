using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LogisticsPlatform.Infrastructure.Services;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    public const string TokenTypeClaim = "typ";
    public const string AccessTokenType = "access";
    public const string RefreshTokenType = "refresh";

    /// <summary>
    /// Keep long ClaimTypes.* names in the JWT so they match JwtBearer
    /// (MapInboundClaims = false + RoleClaimType/NameClaimType = ClaimTypes.*).
    /// </summary>
    private static readonly JwtSecurityTokenHandler TokenHandler = CreateTokenHandler();

    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    private static JwtSecurityTokenHandler CreateTokenHandler()
    {
        var handler = new JwtSecurityTokenHandler();
        handler.OutboundClaimTypeMap.Clear();
        return handler;
    }

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(TokenTypeClaim, AccessTokenType)
        };

        foreach (string role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return CreateToken(claims, DateTime.UtcNow.AddMinutes(_jwtOptions.AccessExpirationInMinutes));
    }

    public string GenerateRefreshToken(Guid userId)
    {
        Span<byte> nonce = stackalloc byte[32];
        RandomNumberGenerator.Fill(nonce);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(TokenTypeClaim, RefreshTokenType),
            new("nonce", Convert.ToBase64String(nonce))
        ];

        return CreateToken(claims, DateTime.UtcNow.AddDays(_jwtOptions.RefreshExpirationInDays));
    }

    public bool TryValidateRefreshToken(string refreshToken, out ClaimsPrincipal? principal)
    {
        principal = null;
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        try
        {
            principal = TokenHandler.ValidateToken(refreshToken, ValidationParameters(), out _);

            string? typ = principal.Claims.FirstOrDefault(c => c.Type == TokenTypeClaim)?.Value
                ?? principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Typ)?.Value;

            if (!string.Equals(typ, RefreshTokenType, StringComparison.Ordinal))
            {
                principal = null;
                return false;
            }

            return true;
        }
        catch
        {
            principal = null;
            return false;
        }
    }

    public string HashRefreshToken(string token)
    {
        byte[] data = Encoding.UTF8.GetBytes(token);
        byte[] hash = SHA256.HashData(data);
        return Convert.ToHexString(hash);
    }

    public string GenerateToken(ApplicationUser user, IList<string> roles) =>
        GenerateAccessToken(user, roles);

    private string CreateToken(IEnumerable<Claim> claims, DateTime expiresUtc)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresUtc,
            signingCredentials: creds);

        return TokenHandler.WriteToken(token);
    }

    private TokenValidationParameters ValidationParameters() =>
        new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
}
