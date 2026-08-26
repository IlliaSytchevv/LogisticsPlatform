using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Payments;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
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
    : ICommandHandler<CreateOrderCheckoutCommand, Result<CreateOrderCheckoutResponse>>
{
    private const string Currency = "usd";

    public async Task<Result<CreateOrderCheckoutResponse>> Handle(
        CreateOrderCheckoutCommand command,
        CancellationToken cancellationToken)
    {
        StripeOptions options = stripeOptions.Value;
        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            return Result<CreateOrderCheckoutResponse>.Invalid(
            [
                new ValidationError("Stripe", "Stripe SecretKey is not configured.")
            ]);
        }

        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<CreateOrderCheckoutResponse>.NotFound();

        // Short RedLock: only for the race around acquire + create pending.
        string mutexKey = $"payment:mutex:{command.OrderId:D}";
        await using IDistributedLockHandle mutex = await redisLock.AcquireAsync(
            mutexKey,
            expiry: TimeSpan.FromSeconds(30));

        if (!mutex.IsAcquired)
        {
            return Result<CreateOrderCheckoutResponse>.Conflict(
                "Payment already in progress for this order. Try again in a moment.");
        }

        if (await orderPaymentsRepository.HasPaidAsync(command.OrderId, cancellationToken))
            return Result<CreateOrderCheckoutResponse>.Conflict("Order is already paid.");

        // Long-lived lock: second user blocked while first is on Stripe (up to TTL).
        if (!await orderCheckoutLock.TryAcquireAsync(command.OrderId, cancellationToken))
        {
            return Result<CreateOrderCheckoutResponse>.Conflict(
                "Payment already in progress for this order. Try again in a moment.");
        }

        try
        {
            string? orderNumber =
                await orderPaymentsRepository.GetOrderNumberAsync(command.OrderId, cancellationToken);
            if (orderNumber is null)
            {
                await orderCheckoutLock.ReleaseAsync(command.OrderId, cancellationToken);
                return Result<CreateOrderCheckoutResponse>.NotFound();
            }

            long amountCents =
                await orderPaymentsRepository.SumActiveSuppliesCentsAsync(command.OrderId, cancellationToken);
            if (amountCents <= 0)
            {
                await orderCheckoutLock.ReleaseAsync(command.OrderId, cancellationToken);
                return Result<CreateOrderCheckoutResponse>.Invalid(
                [
                    new ValidationError(
                        "Amount",
                        "Order has no billable supplies. Add supplies with a price before paying.")
                ]);
            }

            var payment = await orderPaymentsRepository.CreatePendingAsync(
                command.OrderId,
                amountCents,
                Currency,
                cancellationToken);

            await orderPaymentsRepository.CancelPendingExceptAsync(
                command.OrderId,
                payment.Id,
                cancellationToken);

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
                return Result<CreateOrderCheckoutResponse>.Error($"Stripe checkout failed: {ex.Message}");
            }

            await orderPaymentsRepository.SetStripeSessionIdAsync(
                payment.Id,
                session.SessionId,
                cancellationToken);

            // Keep orderCheckoutLock until webhook MarkPaid (or TTL).
            return Result.Success(
                new CreateOrderCheckoutResponse(payment.Id, session.Url, amountCents, Currency));
        }
        catch
        {
            await orderCheckoutLock.ReleaseAsync(command.OrderId, cancellationToken);
            throw;
        }
    }
}
