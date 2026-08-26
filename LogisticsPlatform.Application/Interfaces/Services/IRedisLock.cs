namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IDistributedLockHandle : IAsyncDisposable
{
    bool IsAcquired { get; }
}

public interface IRedisLock
{
    Task<IDistributedLockHandle> AcquireAsync(
        string key,
        TimeSpan? expiry = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null);
}