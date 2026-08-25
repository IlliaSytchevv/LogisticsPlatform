using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Infrastructure.Redis;
using Microsoft.Extensions.Logging;

namespace LogisticsPlatform.Infrastructure.Services;

public sealed class NotificationsFeedCacheInvalidator(
    IRedisService redis,
    ILogger<NotificationsFeedCacheInvalidator> logger) : INotificationsFeedCacheInvalidator
{
    public async Task InvalidateAsync(CancellationToken cancellationToken = default)
    {
        string version = Guid.NewGuid().ToString("N");
        
        await redis.SetDataAsync(NotificationsFeedCacheRedisKeys.VersionKey, version);
        
        logger.LogDebug("Notifications feed cache invalidated, version={Version}", version);
    }
}