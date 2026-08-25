namespace LogisticsPlatform.Infrastructure.Redis;

public static class NotificationsFeedCacheRedisKeys
{
    private const string Prefix = "Logistics_notifications_feed";
    public const string VersionKey = Prefix + ":version";

    public static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);

    public static string Entry(string version, int days, int take)
    {
        return $"{Prefix}:v{version}:d{days}:t{take}";
    }
}