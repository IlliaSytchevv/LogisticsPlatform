using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Payments.HandleWebhook;

public sealed class HandleStripeWebhookCommandHandler(
    IStripeCheckoutService stripeCheckoutService,
    IOrderPaymentsRepository orderPaymentsRepository,
    IOrderCheckoutLock orderCheckoutLock)
    : ICommandHandler<HandleStripeWebhookCommand, Result>
{
    public async Task<Result> Handle(
        HandleStripeWebhookCommand command,
        CancellationToken cancellationToken)
    {
        StripeWebhookEventResult parsed = stripeCheckoutService.ParseWebhookEvent(
            command.JsonBody,
            command.StripeSignatureHeader);

        if (!parsed.IsValid)
        {
            return Result.Invalid(
            [
                new ValidationError("Stripe-Signature", parsed.ErrorMessage ?? "Invalid webhook signature.")
            ]);
        }

        if (!string.Equals(parsed.EventType, "checkout.session.completed", StringComparison.Ordinal))
            return Result.Success();

        if (string.IsNullOrWhiteSpace(parsed.SessionId))
        {
            return Result.Invalid(
            [
                new ValidationError("SessionId", "Checkout session id is missing on webhook event.")
            ]);
        }

        var payment = await orderPaymentsRepository.GetByStripeSessionIdAsync(
            parsed.SessionId,
            cancellationToken);

        if (payment is null)
            return Result.Success();

        if (payment.Status == OrderPaymentStatus.Paid)
        {
            await orderCheckoutLock.ReleaseAsync(payment.OrderId, cancellationToken);
            return Result.Success();
        }

        await orderPaymentsRepository.MarkPaidAsync(payment.Id, cancellationToken);
        await orderCheckoutLock.ReleaseAsync(payment.OrderId, cancellationToken);

        return Result.Success();
    }
}
