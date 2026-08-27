using System.Collections.Concurrent;
using System.Text.Json;
using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticPlatform.IntegrationTests.Helpers;

public sealed class FakeStripeCheckoutService : IStripeCheckoutService
{
    public const string TestSignature = "integration-test-signature";

    private readonly ConcurrentDictionary<string, OpenSession> _openSessions = new();

    public Task<StripeCheckoutSessionResult> CreateCheckoutSessionAsync(
        StripeCheckoutSessionRequest request,
        CancellationToken cancellationToken)
    {
        string sessionId = $"cs_test_{Guid.NewGuid():N}";
        _openSessions[sessionId] = new OpenSession(request.OrderId, request.PaymentId, request.AmountCents);

        return Task.FromResult(new StripeCheckoutSessionResult(
            sessionId,
            $"https://checkout.test/{sessionId}"));
    }

    public Task<string?> TryGetOpenCheckoutUrlAsync(
        string stripeSessionId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            _openSessions.ContainsKey(stripeSessionId)
                ? $"https://checkout.test/{stripeSessionId}"
                : null);
    }

    public Task ExpireCheckoutSessionAsync(string stripeSessionId, CancellationToken cancellationToken)
    {
        _openSessions.TryRemove(stripeSessionId, out _);
        return Task.CompletedTask;
    }

    public StripeWebhookEventResult ParseWebhookEvent(string json, string stripeSignatureHeader)
    {
        if (!string.Equals(stripeSignatureHeader, TestSignature, StringComparison.Ordinal))
        {
            return new StripeWebhookEventResult(false, "Invalid test webhook signature.", null, null, null, null);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            string? eventType = root.GetProperty("type").GetString();
            string? sessionId = root.GetProperty("sessionId").GetString();

            Guid? paymentId = null;
            if (root.TryGetProperty("paymentId", out JsonElement paymentIdElement)
                && Guid.TryParse(paymentIdElement.GetString(), out Guid parsedPaymentId))
            {
                paymentId = parsedPaymentId;
            }

            Guid? orderId = null;
            if (root.TryGetProperty("orderId", out JsonElement orderIdElement)
                && Guid.TryParse(orderIdElement.GetString(), out Guid parsedOrderId))
            {
                orderId = parsedOrderId;
            }

            return new StripeWebhookEventResult(true, null, eventType, sessionId, paymentId, orderId);
        }
        catch (JsonException ex)
        {
            return new StripeWebhookEventResult(false, ex.Message, null, null, null, null);
        }
    }

    private sealed record OpenSession(Guid OrderId, Guid PaymentId, long AmountCents);
}
