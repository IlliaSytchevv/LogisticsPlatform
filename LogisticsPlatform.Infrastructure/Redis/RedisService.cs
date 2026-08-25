using System.Text.Json;
using LogisticsPlatform.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace LogisticsPlatform.Infrastructure.Redis;

public sealed class RedisService(
    IConnectionMultiplexer multiplexer,
    ILogger<RedisService> logger) : IRedisService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private IDatabase Db => multiplexer.GetDatabase();

    public async Task<string?> GetDataAsync(string key)
    {
        RedisValue value = await Db.StringGetAsync(key);
        
        return value.HasValue ? value.ToString() : null;
    }

    public Task SetDataAsync(string key, string value, TimeSpan? expiry = null)
    {
        return Db.StringSetAsync(key, value, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        string? json = await GetDataAsync(key);
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to deserialize Redis key {Key}", key);
            return default;
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        string json = JsonSerializer.Serialize(value, JsonOptions);
        
        return SetDataAsync(key, json, expiry);
    }

    public Task DeleteDataAsync(string key)
    {
        return Db.KeyDeleteAsync(key);
    }
}
