using LogisticsPlatform.Application.Interfaces.Services;
using StackExchange.Redis;

namespace LogisticsPlatform.Infrastructure.Redis;

public sealed class OrderEditLock(IConnectionMultiplexer multiplexer) : IOrderEditLock
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(45);

    private IDatabase Db => multiplexer.GetDatabase();

    public async Task<bool> TryAcquireAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        string key = Key(orderId);
        string token = userId.ToString("D");

        RedisValue current = await Db.StringGetAsync(key);
        if (current.HasValue)
        {
            if (string.Equals(current.ToString(), token, StringComparison.Ordinal))
            {
                await Db.KeyExpireAsync(key, Ttl);
                return true;
            }

            return false;
        }

        return await Db.StringSetAsync(key, token, Ttl, When.NotExists);
    }

    public async Task<bool> HeartbeatAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        string key = Key(orderId);
        string token = userId.ToString("D");

        RedisValue current = await Db.StringGetAsync(key);
        if (!current.HasValue || !string.Equals(current.ToString(), token, StringComparison.Ordinal))
        {
            return false;
        }

        return await Db.KeyExpireAsync(key, Ttl);
    }

    public async Task ReleaseAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        string key = Key(orderId);
        string token = userId.ToString("D");

        RedisValue current = await Db.StringGetAsync(key);
        if (!current.HasValue || !string.Equals(current.ToString(), token, StringComparison.Ordinal))
        {
            return;
        }

        await Db.KeyDeleteAsync(key);
    }

    private static string Key(Guid orderId)
    {
        return $"order-edit:{orderId:D}";
    }
}