namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IStripeCheckoutService
{
    Task<StripeCheckoutSessionResult> CreateCheckoutSessionAsync(
        StripeCheckoutSessionRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns the hosted checkout URL when the session is still open; otherwise null.
    /// </summary>
    Task<string?> TryGetOpenCheckoutUrlAsync(
        string stripeSessionId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Best-effort expire. No-ops when the session is already complete/expired.
    /// </summary>
    Task ExpireCheckoutSessionAsync(
        string stripeSessionId,
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
