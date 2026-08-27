namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IOrderCheckoutLock
{
    Task<bool> TryAcquireAsync(Guid orderId, CancellationToken cancellationToken);

    Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken);
}
