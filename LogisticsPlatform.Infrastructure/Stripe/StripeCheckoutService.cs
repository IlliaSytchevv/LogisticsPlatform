using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace LogisticsPlatform.Infrastructure.Stripe;

public sealed class StripeCheckoutService(
    IOptions<StripeOptions> stripeOptions,
    ILogger<StripeCheckoutService> logger) : IStripeCheckoutService
{
    public async Task<StripeCheckoutSessionResult> CreateCheckoutSessionAsync(
        StripeCheckoutSessionRequest request,
        CancellationToken cancellationToken)
    {
        EnsureApiKey();

        var sessionOptions = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            ClientReferenceId = request.OrderId.ToString(),
            Metadata = new Dictionary<string, string>
            {
                ["orderId"] = request.OrderId.ToString(),
                ["paymentId"] = request.PaymentId.ToString()
            },
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency,
                        UnitAmount = request.AmountCents,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Order {request.OrderNumber} — supplies"
                        }
                    }
                }
            ]
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(sessionOptions, cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(session.Url))
        {
            throw new InvalidOperationException("Stripe did not return a checkout URL.");
        }

        return new StripeCheckoutSessionResult(session.Id, session.Url);
    }

    public async Task<string?> TryGetOpenCheckoutUrlAsync(
        string stripeSessionId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(stripeSessionId))
        {
            return null;
        }

        EnsureApiKey();

        try
        {
            var service = new SessionService();
            Session session = await service.GetAsync(stripeSessionId, cancellationToken: cancellationToken);

            if (!string.Equals(session.Status, "open", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return string.IsNullOrWhiteSpace(session.Url) ? null : session.Url;
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Failed to load Stripe checkout session {SessionId}", stripeSessionId);
            return null;
        }
    }

    public async Task ExpireCheckoutSessionAsync(
        string stripeSessionId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(stripeSessionId))
        {
            return;
        }

        EnsureApiKey();

        try
        {
            var service = new SessionService();
            await service.ExpireAsync(stripeSessionId, cancellationToken: cancellationToken);
        }
        catch (StripeException ex)
        {
            logger.LogInformation(ex, "Stripe session {SessionId} was not expired (likely already closed)", stripeSessionId);
        }
    }

    public StripeWebhookEventResult ParseWebhookEvent(string json, string stripeSignatureHeader)
    {
        StripeOptions options = stripeOptions.Value;
        if (string.IsNullOrWhiteSpace(options.WebhookSecret))
        {
            return new StripeWebhookEventResult(false, "Stripe WebhookSecret is not configured.",
                null, null, null, null);
        }

        try
        {
            Event stripeEvent = EventUtility.ConstructEvent(json, stripeSignatureHeader, options.WebhookSecret);

            if (stripeEvent.Data.Object is not Session session)
            {
                return new StripeWebhookEventResult(true, null, stripeEvent.Type, 
                    null, null, null);
            }

            Guid? paymentId = null;
            Guid? orderId = null;

            if (session.Metadata is not null)
            {
                if (session.Metadata.TryGetValue("paymentId", out string? paymentIdRaw) &&
                    Guid.TryParse(paymentIdRaw, out Guid parsedPaymentId))
                {
                    paymentId = parsedPaymentId;
                }

                if (session.Metadata.TryGetValue("orderId", out string? orderIdRaw) &&
                    Guid.TryParse(orderIdRaw, out Guid parsedOrderId))
                {
                    orderId = parsedOrderId;
                }
            }

            return new StripeWebhookEventResult(true, null, stripeEvent.Type,
                session.Id, paymentId, orderId);
        }
        catch (StripeException ex)
        {
            return new StripeWebhookEventResult(
                false,
                ex.Message,
                null,
                null,
                null,
                null);
        }
    }

    private void EnsureApiKey()
    {
        StripeConfiguration.ApiKey = stripeOptions.Value.SecretKey;
    }
}
