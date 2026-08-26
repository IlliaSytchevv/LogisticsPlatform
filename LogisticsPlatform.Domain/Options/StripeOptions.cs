namespace LogisticsPlatform.Domain.Options;

public class StripeOptions
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; init; } = string.Empty;
    public string WebhookSecret { get; init; } = string.Empty;

    /// <summary>Use {orderId} placeholder. Example: http://localhost:3000/orders/{orderId}?payment=success</summary>
    public string SuccessUrlTemplate { get; init; } = "http://localhost:3000/orders/{orderId}?payment=success";

    /// <summary>Use {orderId} placeholder. Example: http://localhost:3000/orders/{orderId}?payment=cancel</summary>
    public string CancelUrlTemplate { get; init; } = "http://localhost:3000/orders/{orderId}?payment=cancel";
}
