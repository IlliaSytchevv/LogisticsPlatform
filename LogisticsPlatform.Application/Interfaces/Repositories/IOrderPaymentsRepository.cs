using LogisticsPlatform.Application.Models.Payments;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderPaymentsRepository
{
    Task<string?> GetOrderNumberAsync(Guid orderId, CancellationToken cancellationToken);

    Task<long> SumActiveSuppliesCentsAsync(Guid orderId, CancellationToken cancellationToken);

    Task<bool> HasPaidAsync(Guid orderId, CancellationToken cancellationToken);

    Task<(OrderType Type, OrderStatus Status)?> GetTypeAndStatusAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<OrderPaymentData> CreatePendingAsync(
        Guid orderId,
        long amountCents,
        string currency,
        CancellationToken cancellationToken);

    Task SetStripeSessionIdAsync(
        Guid paymentId,
        string stripeSessionId,
        CancellationToken cancellationToken);

    Task<OrderPaymentData?> GetLatestPendingWithSessionAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<OrderPaymentData?> GetByStripeSessionIdAsync(
        string stripeSessionId,
        CancellationToken cancellationToken);

    Task<OrderPaymentData?> GetByIdAsync(
        Guid paymentId,
        CancellationToken cancellationToken);

    Task<bool> MarkPaidAsync(
        Guid paymentId,
        CancellationToken cancellationToken);
    
    Task<bool> MarkCanceledIfPendingAsync(
        Guid paymentId,
        CancellationToken cancellationToken);
    
    Task<IReadOnlyList<string>> CancelPendingExceptAsync(
        Guid orderId,
        Guid keepPaymentId,
        CancellationToken cancellationToken);
}
