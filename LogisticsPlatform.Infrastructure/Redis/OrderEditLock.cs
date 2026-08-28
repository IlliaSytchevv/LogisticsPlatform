using LogisticsPlatform.Application.Interfaces.Services;
using StackExchange.Redis;

namespace LogisticsPlatform.Infrastructure.Redis;

public sealed class OrderEditLock(IConnectionMultiplexer multiplexer) : IOrderEditLock
{
    private IDatabase Db => multiplexer.GetDatabase();
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(45);

    // Only extend the lock if it's still yours (atomic: no race between read and expire).
    private const string RenewIfOwnerScript = """
        if redis.call('GET', KEYS[1]) == ARGV[1] then
          return redis.call('EXPIRE', KEYS[1], ARGV[2])
        end
        return 0
        """;

    // Only release the lock if it's still yours (atomic: won't clear another user's lock).
    private const string ReleaseIfOwnerScript = """
        if redis.call('GET', KEYS[1]) == ARGV[1] then
          return redis.call('DEL', KEYS[1])
        end
        return 0
        """;

    public async Task<bool> TryAcquireAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        string key = Key(orderId);
        string token = userId.ToString("D");

        if (await Db.StringSetAsync(key, token, Ttl, When.NotExists))
        {
            return true;
        }

        return false;
    }

    public async Task<bool> HeartbeatAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        string key = Key(orderId);
        string token = userId.ToString("D");
        int ttlSeconds = (int)Ttl.TotalSeconds;

        RedisResult renewed = await Db.ScriptEvaluateAsync(
            RenewIfOwnerScript,
            [key],
            [token, ttlSeconds]);

        return (int)renewed == 1;
    }

    public async Task<bool> IsHeldByAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        string key = Key(orderId);
        string token = userId.ToString("D");

        RedisValue current = await Db.StringGetAsync(key);
        
        return current.HasValue && string.Equals(current.ToString(), token, StringComparison.Ordinal);
    }

    public async Task ReleaseAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        string key = Key(orderId);
        string token = userId.ToString("D");

        await Db.ScriptEvaluateAsync(
            ReleaseIfOwnerScript,
            [key],
            [token]);
    }

    private static string Key(Guid orderId)
    {
        return $"order-edit:{orderId:D}";
    }
}