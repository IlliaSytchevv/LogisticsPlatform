using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Notifications;
using LogisticsPlatform.Application.UseCases.Notifications.GetFeed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Authorize]
[Route(ApiRoutes.ApiRoutes.Notifications)]
public sealed class NotificationsController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] NotificationsFeedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(
            new GetNotificationsFeedQuery(request.Days, request.Take),
            cancellationToken);

        return GetActionResult(result);
    }
}