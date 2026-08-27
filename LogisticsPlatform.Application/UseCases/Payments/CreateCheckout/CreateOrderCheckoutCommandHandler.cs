using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Payments;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Payments;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Options;

namespace LogisticsPlatform.Application.UseCases.Payments.CreateCheckout;

public sealed class CreateOrderCheckoutCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderPaymentsRepository orderPaymentsRepository,
    IStripeCheckoutService stripeCheckoutService,
    IRedisLock redisLock,
    IOrderCheckoutLock orderCheckoutLock,
    IOptions<StripeOptions> stripeOptions)
    : ICommandHandler<CreateOrderCheckoutCommand, Result<CreateCheckoutResponse>>
{
    private const string Currency = "usd";

    public async Task<Result<CreateCheckoutResponse>> Handle(
        CreateOrderCheckoutCommand command,
        CancellationToken cancellationToken)
    {
        StripeOptions options = stripeOptions.Value;
        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            return Result<CreateCheckoutResponse>.Invalid(
            [
                new ValidationError("Stripe", "Stripe SecretKey is not configured.")
            ]);
        }

        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<CreateCheckoutResponse>.NotFound();

        string mutexKey = $"payment:mutex:{command.OrderId:D}";
        await using IDistributedLockHandle mutex = await redisLock.AcquireAsync(
            mutexKey,
            expiry: TimeSpan.FromSeconds(30));

        if (!mutex.IsAcquired)
        {
            return Result<CreateCheckoutResponse>.Conflict("Try again in a moment.");
        }

        if (await orderPaymentsRepository.HasPaidAsync(command.OrderId, cancellationToken))
            return Result<CreateCheckoutResponse>.Conflict("Order is already paid.");

        var typeAndStatus = await orderPaymentsRepository.GetTypeAndStatusAsync(command.OrderId, cancellationToken);
        if (typeAndStatus is null)
            return Result<CreateCheckoutResponse>.NotFound();

        if (typeAndStatus.Value.Status is OrderStatus.Draft or OrderStatus.Closed)
        {
            return Result<CreateCheckoutResponse>.Invalid(
            [
                new ValidationError("Status", "You cannot pay for an order in Draft or Closed status.")
            ]);
        }

        if (!await orderCheckoutLock.TryAcquireAsync(command.OrderId, cancellationToken))
        {
            return Result<CreateCheckoutResponse>.Conflict("Try again in a moment.");
        }

        try
        {
            string? orderNumber =
                await orderPaymentsRepository.GetOrderNumberAsync(command.OrderId, cancellationToken);
            if (orderNumber is null)
            {
                await orderCheckoutLock.ReleaseAsync(command.OrderId, cancellationToken);
                return Result<CreateCheckoutResponse>.NotFound();
            }

            long amountCents =
                await orderPaymentsRepository.SumActiveSuppliesCentsAsync(command.OrderId, cancellationToken);
            if (amountCents <= 0)
            {
                await orderCheckoutLock.ReleaseAsync(command.OrderId, cancellationToken);
                return Result<CreateCheckoutResponse>.Invalid(
                [
                    new ValidationError(
                        "Amount",
                        "Order has no billable supplies. Add supplies with a price before paying.")
                ]);
            }

            OrderPaymentData? existingPending = await orderPaymentsRepository.GetLatestPendingWithSessionAsync(
                    command.OrderId,
                    cancellationToken);

            if (existingPending is { StripeSessionId: { Length: > 0 } sessionId }
                && existingPending.AmountCents == amountCents
                && string.Equals(existingPending.Currency, Currency, StringComparison.OrdinalIgnoreCase))
            {
                string? openUrl = await stripeCheckoutService.TryGetOpenCheckoutUrlAsync(
                    sessionId,
                    cancellationToken);

                if (openUrl is not null)
                {
                    return Result.Success(new CreateCheckoutResponse(
                            existingPending.Id, openUrl,
                            amountCents, Currency));
                }
            }

            OrderPaymentData payment = await orderPaymentsRepository.CreatePendingAsync(
                command.OrderId,
                amountCents,
                Currency,
                cancellationToken);

            IReadOnlyList<string> sessionsToExpire =
                await orderPaymentsRepository.CancelPendingExceptAsync(
                    command.OrderId,
                    payment.Id,
                    cancellationToken);

            foreach (string oldSessionId in sessionsToExpire)
            {
                await stripeCheckoutService.ExpireCheckoutSessionAsync(oldSessionId, cancellationToken);
            }

            string successUrl = options.SuccessUrlTemplate.Replace(
                "{orderId}",
                command.OrderId.ToString(),
                StringComparison.OrdinalIgnoreCase);
            string cancelUrl = options.CancelUrlTemplate.Replace(
                "{orderId}",
                command.OrderId.ToString(),
                StringComparison.OrdinalIgnoreCase);

            StripeCheckoutSessionResult session;
            try
            {
                session = await stripeCheckoutService.CreateCheckoutSessionAsync(
                    new StripeCheckoutSessionRequest(
                        command.OrderId,
                        payment.Id,
                        orderNumber,
                        amountCents,
                        Currency,
                        successUrl,
                        cancelUrl),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                await orderCheckoutLock.ReleaseAsync(command.OrderId, cancellationToken);
                return Result<CreateCheckoutResponse>.Error($"Stripe checkout failed: {ex.Message}");
            }

            try
            {
                await orderPaymentsRepository.SetStripeSessionIdAsync(
                    payment.Id,
                    session.SessionId,
                    cancellationToken);
            }
            catch
            {
                await stripeCheckoutService.ExpireCheckoutSessionAsync(session.SessionId, cancellationToken);
                throw;
            }

            return Result.Success(new CreateCheckoutResponse(payment.Id, session.Url, amountCents, Currency));
        }
        catch
        {
            await orderCheckoutLock.ReleaseAsync(command.OrderId, cancellationToken);
            throw;
        }
    }
}
