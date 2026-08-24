using System.Security.Claims;
using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Application.Models.Auth;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Options;
using RefreshTokenEntity = LogisticsPlatform.Domain.Entities.RefreshToken;

namespace LogisticsPlatform.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IJwtTokenService jwtTokenService,
    IRefreshTokenStore refreshTokenStore,
    IUserManagerWrapper userManagerWrapper,
    IOptions<JwtOptions> jwtOptions)
    : ICommandHandler<RefreshTokenCommand, Result<RefreshTokensData>>
{
    public async Task<Result<RefreshTokensData>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshTokenFromCookie))
            return Result<RefreshTokensData>.Unauthorized();

        if (!jwtTokenService.TryValidateRefreshToken(command.RefreshTokenFromCookie, out ClaimsPrincipal? principal)
            || principal is null)
        {
            return Result<RefreshTokensData>.Unauthorized();
        }

        string oldHash = jwtTokenService.HashRefreshToken(command.RefreshTokenFromCookie);
        DateTime now = DateTime.UtcNow;

        var stored = await refreshTokenStore.FindByHashAsync(oldHash, cancellationToken);
        if (stored is null || stored.IsRevoked || stored.ExpiryDateUtc <= now)
            return Result<RefreshTokensData>.Unauthorized();

        ApplicationUser? user = await userManagerWrapper.FindByIdAsync(stored.UserId.ToString());
        if (user is null || !user.IsActive)
            return Result<RefreshTokensData>.Unauthorized();

        var roles = await userManagerWrapper.GetRolesAsync(user);
        string newAccessToken = jwtTokenService.GenerateAccessToken(user, roles);
        string newRefreshToken = jwtTokenService.GenerateRefreshToken(user.Id);
        string newHash = jwtTokenService.HashRefreshToken(newRefreshToken);
        DateTime newExpiryUtc = now.AddDays(jwtOptions.Value.RefreshExpirationInDays);

        bool revoked = await refreshTokenStore.TryRevokeAndReplaceAsync(
            stored.Id,
            newHash,
            now,
            cancellationToken);

        if (!revoked)
            return Result<RefreshTokensData>.Unauthorized();

        await refreshTokenStore.AddAsync(
            new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newHash,
                CreatedAtUtc = now,
                ExpiryDateUtc = newExpiryUtc,
                IsRevoked = false
            },
            cancellationToken);

        return Result.Success(new RefreshTokensData(newAccessToken, newRefreshToken, newExpiryUtc));
    }
}
