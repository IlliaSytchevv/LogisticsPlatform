using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticsPlatform.Application.UseCases.Auth.Logout;

public sealed class LogoutCommandHandler(
    IJwtTokenService jwtTokenService,
    IRefreshTokenStore refreshTokenStore)
    : ICommandHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshTokenFromCookie))
            return Result.Success();

        string hash = jwtTokenService.HashRefreshToken(command.RefreshTokenFromCookie);
        await refreshTokenStore.RevokeByHashAsync(hash, cancellationToken);
        return Result.Success();
    }
}
