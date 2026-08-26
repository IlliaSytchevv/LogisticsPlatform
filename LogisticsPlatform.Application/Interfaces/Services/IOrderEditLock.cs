namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IOrderEditLock
{
    Task<bool> TryAcquireAsync(Guid orderId, Guid userId, CancellationToken cancellationToken);

    Task<bool> HeartbeatAsync(Guid orderId, Guid userId, CancellationToken cancellationToken);

    Task ReleaseAsync(Guid orderId, Guid userId, CancellationToken cancellationToken);
}
