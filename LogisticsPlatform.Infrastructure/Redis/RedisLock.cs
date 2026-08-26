using LogisticsPlatform.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;

namespace LogisticsPlatform.Infrastructure.Redis;

public sealed class RedisLock(RedLockFactory factory, ILogger<RedisLock> logger) : IRedisLock
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan DefaultWait = TimeSpan.Zero;
    private static readonly TimeSpan DefaultRetry = TimeSpan.FromMilliseconds(200);

    public async Task<IDistributedLockHandle> AcquireAsync(
        string key,
        TimeSpan? expiry = null,
        TimeSpan? waitTime = null,
        TimeSpan? retryTime = null)
    {
        TimeSpan expiryTime = expiry ?? DefaultExpiry;
        TimeSpan wait = waitTime ?? DefaultWait;
        TimeSpan retry = retryTime ?? DefaultRetry;

        IRedLock redLock = await factory.CreateLockAsync(
            resource: key,
            expiryTime: expiryTime,
            waitTime: wait,
            retryTime: retry);

        if (redLock.IsAcquired)
        {
            logger.LogInformation("Lock acquired for key {Key}, expires in {ExpirySeconds}s", key, expiryTime.TotalSeconds);
        }
        else
        {
            logger.LogWarning("Failed to acquire lock for key {Key}", key);
        }

        return new RedLockHandle(redLock);
    }

    private sealed class RedLockHandle(IRedLock redLock) : IDistributedLockHandle
    {
        public bool IsAcquired => redLock.IsAcquired;

        public ValueTask DisposeAsync()
        {
            redLock.Dispose();
            
            return ValueTask.CompletedTask;
        }
    }
}