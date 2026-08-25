namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IRedisService
{
    Task<string?> GetDataAsync(string key);

    Task SetDataAsync(string key, string value, TimeSpan? expiry = null);

    Task<T?> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    Task DeleteDataAsync(string key);
}