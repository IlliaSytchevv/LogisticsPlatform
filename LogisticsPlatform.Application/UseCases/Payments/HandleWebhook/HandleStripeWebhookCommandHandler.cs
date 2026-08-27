using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Payments;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Payments.HandleWebhook;

public sealed class HandleStripeWebhookCommandHandler(
    IStripeCheckoutService stripeCheckoutService,
    IOrderPaymentsRepository orderPaymentsRepository,
    IOrderCheckoutLock orderCheckoutLock)
    : ICommandHandler<HandleStripeWebhookCommand, Result>
{
    private const string CheckoutSessionCompleted = "checkout.session.completed";
    private const string CheckoutSessionExpired = "checkout.session.expired";

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

        bool isCompleted = string.Equals(parsed.EventType, CheckoutSessionCompleted, StringComparison.Ordinal);
        bool isExpired = string.Equals(parsed.EventType, CheckoutSessionExpired, StringComparison.Ordinal);

        if (!isCompleted && !isExpired)
            return Result.Success();

        if (string.IsNullOrWhiteSpace(parsed.SessionId))
        {
            return Result.Invalid(
            [
                new ValidationError("SessionId", "Checkout session id is missing on webhook event.")
            ]);
        }

        OrderPaymentData? payment = await ResolvePaymentAsync(parsed, cancellationToken);

        if (payment is null)
        {
            if (isCompleted)
            {
                return Result.CriticalError($"No payment found for Stripe session {parsed.SessionId} (session id or metadata paymentId).");
            }

            return Result.Success();
        }

        if (isCompleted)
        {
            if (payment.Status == OrderPaymentStatus.Paid)
            {
                await orderCheckoutLock.ReleaseAsync(payment.OrderId, cancellationToken);
                return Result.Success();
            }

            await orderPaymentsRepository.MarkPaidAsync(payment.Id, cancellationToken);
            await orderCheckoutLock.ReleaseAsync(payment.OrderId, cancellationToken);

            return Result.Success();
        }

        await orderPaymentsRepository.MarkCanceledIfPendingAsync(payment.Id, cancellationToken);
        await orderCheckoutLock.ReleaseAsync(payment.OrderId, cancellationToken);

        return Result.Success();
    }

    private async Task<OrderPaymentData?> ResolvePaymentAsync(
        StripeWebhookEventResult parsed,
        CancellationToken cancellationToken)
    {
        OrderPaymentData? payment = await orderPaymentsRepository.GetByStripeSessionIdAsync(
            parsed.SessionId!,
            cancellationToken);

        if (payment is not null)
        {
            return payment;
        }

        if (parsed.PaymentId is not Guid paymentId)
        {
            return null;
        }

        payment = await orderPaymentsRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
        {
            return null;
        }
        
        if (string.IsNullOrWhiteSpace(payment.StripeSessionId))
        {
            await orderPaymentsRepository.SetStripeSessionIdAsync(
                payment.Id,
                parsed.SessionId!,
                cancellationToken);

            payment = await orderPaymentsRepository.GetByIdAsync(paymentId, cancellationToken);
        }

        return payment;
    }
}