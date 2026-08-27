using LogisticsPlatform.Application.Interfaces.Services;
using StackExchange.Redis;

namespace LogisticsPlatform.Infrastructure.Redis;

public sealed class OrderCheckoutLock(IConnectionMultiplexer multiplexer) : IOrderCheckoutLock
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(20);

    private const string HeldValue = "1";

    private IDatabase Db => multiplexer.GetDatabase();

    public Task<bool> TryAcquireAsync(Guid orderId, CancellationToken cancellationToken) =>
        Db.StringSetAsync(Key(orderId), HeldValue, Ttl, When.NotExists);

    public async Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken)
    {
        await Db.KeyDeleteAsync(Key(orderId));
    }

    private static string Key(Guid orderId) => $"payment-checkout:{orderId:D}";
}
