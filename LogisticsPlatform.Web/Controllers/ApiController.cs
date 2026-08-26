using System.Security.Claims;
using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[ApiController]
public abstract class ApiController(IDispatcher dispatcher) : ControllerBase
{
    protected IDispatcher Dispatcher { get; } = dispatcher;

    protected string? GetCurrentUserDisplayName() =>
        User.FindFirstValue(ClaimTypes.Name);

    protected IActionResult GetActionResult(Result result) =>
        MapStatus(
            result.Status,
            onOk: () => Ok(),
            onCreated: () => StatusCode(StatusCodes.Status201Created),
            validationErrors: result.ValidationErrors,
            errors: result.Errors);

    protected IActionResult GetActionResult<T>(Result<T> result) =>
        MapStatus(
            result.Status,
            onOk: () => Ok(result.Value),
            onCreated: () => StatusCode(StatusCodes.Status201Created, result.Value),
            validationErrors: result.ValidationErrors,
            errors: result.Errors);

    private IActionResult MapStatus(
        ResultStatus status,
        Func<IActionResult> onOk,
        Func<IActionResult> onCreated,
        IEnumerable<ValidationError> validationErrors,
        IEnumerable<string> errors) =>
        status switch
        {
            ResultStatus.Ok => onOk(),
            ResultStatus.Created => onCreated(),
            ResultStatus.NoContent => NoContent(),
            ResultStatus.Invalid => BadRequest(validationErrors),
            ResultStatus.NotFound => NotFound(errors),
            ResultStatus.Unauthorized => Unauthorized(),
            ResultStatus.Forbidden => Forbid(),
            ResultStatus.Conflict => Conflict(errors),
            ResultStatus.Error => BadRequest(errors),
            ResultStatus.CriticalError => StatusCode(StatusCodes.Status500InternalServerError, errors),
            ResultStatus.Unavailable => StatusCode(StatusCodes.Status503ServiceUnavailable, errors),
            _ => StatusCode(StatusCodes.Status500InternalServerError, errors)
        };
}