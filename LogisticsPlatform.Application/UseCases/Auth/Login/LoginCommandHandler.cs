using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Application.Models.Auth;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RefreshTokenEntity = LogisticsPlatform.Domain.Entities.RefreshToken;

namespace LogisticsPlatform.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandler(
    IUserManagerWrapper userManagerWrapper,
    IJwtTokenService jwtTokenService,
    IRefreshTokenStore refreshTokenStore,
    IOptions<JwtOptions> jwtOptions,
    ILogger<LoginCommandHandler> logger)
    : ICommandHandler<LoginCommand, Result<LoginTokensData>>
{
    public async Task<Result<LoginTokensData>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userManagerWrapper.FindByNameAsync(command.Username);
        if (user is null)
        {
            logger.LogWarning("Login failed: user {Username} not found", command.Username);
            return Result<LoginTokensData>.Unauthorized();
        }

        if (!user.IsActive)
        {
            logger.LogWarning("Login failed: user {Username} inactive", command.Username);
            return Result<LoginTokensData>.Unauthorized();
        }

        var passwordValid = await userManagerWrapper.CheckPasswordAsync(user, command.Password);
        if (!passwordValid)
        {
            logger.LogWarning("Login failed: invalid password for {Username}", command.Username);
            return Result<LoginTokensData>.Unauthorized();
        }

        var roles = await userManagerWrapper.GetRolesAsync(user);
        var accessToken = jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = jwtTokenService.GenerateRefreshToken(user.Id);
        var hash = jwtTokenService.HashRefreshToken(refreshToken);
        DateTime expiresUtc = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshExpirationInDays);

        await refreshTokenStore.AddAsync(
            new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = hash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiryDateUtc = expiresUtc,
                IsRevoked = false
            },
            cancellationToken);

        return Result.Success(new LoginTokensData(accessToken, refreshToken, expiresUtc));
    }
}
