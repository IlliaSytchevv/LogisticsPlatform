using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Domain.DTO.Authorization;
using Microsoft.Extensions.Logging;

namespace LogisticsPlatform.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandler(
    IUserManagerWrapper userManagerWrapper,
    IJwtTokenService jwtTokenService,
    ILogger<LoginCommandHandler> logger)
    : ICommandHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userManagerWrapper.FindByNameAsync(command.Username);
        if (user is null)
        {
            logger.LogWarning("Login failed: user {Username} not found", command.Username);
            return Result<LoginResponse>.Unauthorized();
        }

        var passwordValid = await userManagerWrapper.CheckPasswordAsync(user, command.Password);
        if (!passwordValid)
        {
            logger.LogWarning("Login failed: invalid password for {Username}", command.Username);
            return Result<LoginResponse>.Unauthorized();
        }

        var roles = await userManagerWrapper.GetRolesAsync(user);
        var token = jwtTokenService.GenerateToken(user, roles);

        return Result.Success(new LoginResponse(token));
    }
}
