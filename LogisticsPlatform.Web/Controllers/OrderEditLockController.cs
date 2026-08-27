using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.Acquire;
using LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.Heartbeat;
using LogisticsPlatform.Application.UseCases.OrderDetails.EditLock.ReleaseLock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Authorize(Roles = "Admin,Dispatcher")]
[Route(ApiRoutes.ApiRoutes.Order)]
public sealed class OrderEditLockController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpPost("edit-lock")]
    public async Task<IActionResult> AcquireEditLock(Guid orderId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid userId))
        {
            return Unauthorized();
        }

        var result = await Dispatcher.Send(new AcquireOrderEditLockCommand(orderId, userId), cancellationToken);
        
        return GetActionResult(result);
    }

    [HttpPost("edit-lock/heartbeat")]
    public async Task<IActionResult> HeartbeatEditLock(Guid orderId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid userId))
        {
            return Unauthorized();
        }

        var result = await Dispatcher.Send(new HeartbeatOrderEditLockCommand(orderId, userId), cancellationToken);
        
        return GetActionResult(result);
    }

    [HttpDelete("release-edit-lock")]
    public async Task<IActionResult> ReleaseEditLock(Guid orderId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out Guid userId))
        {
            return Unauthorized();
        }

        var result = await Dispatcher.Send(new ReleaseOrderEditLockCommand(orderId, userId), cancellationToken);
        
        return GetActionResult(result);
    }
}