using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.Auth.Login;
using LogisticsPlatform.Application.UseCases.Auth.Logout;
using LogisticsPlatform.Application.UseCases.Auth.RefreshToken;
using LogisticsPlatform.Application.UseCases.Auth.Register;
using LogisticsPlatform.Domain.DTO.Authorization;
using LogisticsPlatform.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Route("api/auth")]
public sealed class AuthController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new RegisterUserCommand(request.Name, request.Email, request.Password, request.Role),
            cancellationToken);

        return GetActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new LoginCommand(request.Username, request.Password),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        RefreshCookie.Set(Response, result.Value.RefreshToken, result.Value.RefreshExpiresUtc);
        return Ok(new AuthTokenResponse(result.Value.AccessToken));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        string? refreshFromCookie = Request.Cookies[RefreshCookie.Name];

        var result = await Dispatcher.Send(
            new RefreshTokenCommand(refreshFromCookie ?? string.Empty),
            cancellationToken);

        if (!result.IsSuccess)
            return GetActionResult(result);

        RefreshCookie.Set(Response, result.Value.RefreshToken, result.Value.RefreshExpiresUtc);
        return Ok(new AuthTokenResponse(result.Value.AccessToken));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        string? refreshFromCookie = Request.Cookies[RefreshCookie.Name];
        
        var result = await Dispatcher.Send(
            new LogoutCommand(refreshFromCookie),
            cancellationToken);

        RefreshCookie.Clear(Response);
        
        return GetActionResult(result);
    }
}
