using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.UseCases.Payments.CreateCheckout;
using LogisticsPlatform.Application.UseCases.Payments.HandleWebhook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Controllers;

[Route(ApiRoutes.ApiRoutes.Payments)]
public sealed class PaymentsController(IDispatcher dispatcher) : ApiController(dispatcher)
{
    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("orders/{orderId:guid}/checkout")]
    public async Task<IActionResult> CreateCheckout(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await Dispatcher.Send(new CreateOrderCheckoutCommand(orderId), cancellationToken);
        
        return GetActionResult(result);
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        string json = await reader.ReadToEndAsync(cancellationToken);
        string signature = Request.Headers["Stripe-Signature"].ToString();

        var result = await Dispatcher.Send(
            new HandleStripeWebhookCommand(json, signature),
            cancellationToken);

        return GetActionResult(result);
    }
}
