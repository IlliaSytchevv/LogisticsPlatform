using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.Notifications.GetFeed;
using LogisticsPlatform.Domain.DTO.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Route("api/notifications")]
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