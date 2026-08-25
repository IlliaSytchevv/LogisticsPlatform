namespace LogisticsPlatform.Domain.Options;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; init; } = "localhost:6379,abortConnect=true";
}