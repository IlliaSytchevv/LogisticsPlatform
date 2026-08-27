using Testcontainers.Redis;

namespace LogisticPlatform.IntegrationTests.Fixtures;

public sealed class RedisContainersFixture : IAsyncLifetime
{
    private RedisContainer _container = null!;

    public string ConnectionString =>
        _container.GetConnectionString()
        ?? throw new InvalidOperationException("Redis test container connection string is unavailable.");

    public async Task InitializeAsync()
    {
        _container = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();

        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
