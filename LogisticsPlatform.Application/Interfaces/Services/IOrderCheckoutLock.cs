namespace LogisticsPlatform.Application.Interfaces.Services;

/// <summary>
/// Long-lived checkout lock: held from CreateCheckout until Paid (or TTL).
/// Prevents a second user starting Stripe while the first payment is still open.
/// </summary>
public interface IOrderCheckoutLock
{
    Task<bool> TryAcquireAsync(Guid orderId, CancellationToken cancellationToken);

    Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken);
}
