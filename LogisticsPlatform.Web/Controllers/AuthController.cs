using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.Auth.Login;
using LogisticsPlatform.Application.UseCases.Auth.Register;
using LogisticsPlatform.Domain.DTO.Authorization;
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

        return GetActionResult(result);
    }
}