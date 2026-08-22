using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[ApiController]
public abstract class ApiController(IDispatcher dispatcher) : ControllerBase
{
    protected IDispatcher Dispatcher { get; } = dispatcher;

    protected IActionResult GetActionResult(Result result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => Ok(),
            ResultStatus.Invalid => BadRequest(result.ValidationErrors),
            ResultStatus.NotFound => NotFound(result.Errors),
            ResultStatus.Unauthorized => Unauthorized(),
            ResultStatus.Forbidden => Forbid(),
            ResultStatus.Error => BadRequest(result.Errors),
            _ => BadRequest(result.Errors)
        };
    }

    protected IActionResult GetActionResult<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => Ok(result.Value),
            ResultStatus.Invalid => BadRequest(result.ValidationErrors),
            ResultStatus.NotFound => NotFound(result.Errors),
            ResultStatus.Unauthorized => Unauthorized(),
            ResultStatus.Forbidden => Forbid(),
            ResultStatus.Error => BadRequest(result.Errors),
            _ => BadRequest(result.Errors)
        };
    }
}