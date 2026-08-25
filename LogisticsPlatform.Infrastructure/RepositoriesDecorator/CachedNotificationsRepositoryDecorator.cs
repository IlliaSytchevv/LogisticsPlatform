using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Notifications;
using LogisticsPlatform.Infrastructure.Redis;
using Microsoft.Extensions.Logging;

namespace LogisticsPlatform.Infrastructure.RepositoriesDecorator;

public sealed class CachedNotificationsRepositoryDecorator(
    INotificationsRepository inner,
    IRedisService redis,
    ILogger<CachedNotificationsRepositoryDecorator> logger) : INotificationsRepository
{
    public async Task<NotificationsFeedData> GetFeedAsync(
        int days,
        int take,
        CancellationToken cancellationToken)
    {
        string version = await redis.GetDataAsync(NotificationsFeedCacheRedisKeys.VersionKey) ?? "0";
        string cacheKey = NotificationsFeedCacheRedisKeys.Entry(version, days, take);

        NotificationsFeedData? cached = await redis.GetAsync<NotificationsFeedData>(cacheKey);
        if (cached is not null)
        {
            logger.LogDebug("Cache hit for notifications feed {CacheKey}", cacheKey);
            return cached;
        }

        logger.LogDebug("Cache miss for notifications feed {CacheKey}", cacheKey);

        NotificationsFeedData data = await inner.GetFeedAsync(days, take, cancellationToken);
        await redis.SetAsync(cacheKey, data, NotificationsFeedCacheRedisKeys.Ttl);
        
        return data;
    }
}