namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IStripeCheckoutService
{
    Task<StripeCheckoutSessionResult> CreateCheckoutSessionAsync(
        StripeCheckoutSessionRequest request,
        CancellationToken cancellationToken);

    StripeWebhookEventResult ParseWebhookEvent(string json, string stripeSignatureHeader);
}

public sealed record StripeCheckoutSessionRequest(
    Guid OrderId,
    Guid PaymentId,
    string OrderNumber,
    long AmountCents,
    string Currency,
    string SuccessUrl,
    string CancelUrl);

public sealed record StripeCheckoutSessionResult(string SessionId, string Url);

public sealed record StripeWebhookEventResult(
    bool IsValid,
    string? ErrorMessage,
    string? EventType,
    string? SessionId,
    Guid? PaymentId,
    Guid? OrderId);
