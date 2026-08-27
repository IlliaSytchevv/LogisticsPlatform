using LogisticsPlatform.Infrastructure.Database.Seed;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LogisticPlatform.IntegrationTests.Fixtures;

public sealed class LogisticsApiFixture : IAsyncLifetime
{
    private readonly PostgresContainersFixture _postgres = new();
    private readonly RedisContainersFixture _redis = new();
    private CustomWebApplicationFactory? _factory;
    private HttpClient? _client;

    public CustomWebApplicationFactory Factory =>
        _factory ?? throw new InvalidOperationException("Fixture was not initialized.");

    public HttpClient Client =>
        _client ?? throw new InvalidOperationException("Fixture was not initialized.");

    public string ConnectionString => _postgres.ConnectionString;

    public async Task InitializeAsync()
    {
        await _postgres.InitializeAsync();
        await _redis.InitializeAsync();

        _factory = new CustomWebApplicationFactory(_postgres.ConnectionString, _redis.ConnectionString);
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });

        await SeedData.InitializeAsync(_factory.Services);
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();

        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }

        await _postgres.DisposeAsync();
        await _redis.DisposeAsync();
    }
}

