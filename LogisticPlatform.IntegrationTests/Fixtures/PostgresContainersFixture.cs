using Testcontainers.PostgreSql;

namespace LogisticPlatform.IntegrationTests.Fixtures;

public sealed class PostgresContainersFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    public string ConnectionString => _container?.GetConnectionString();

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("logistics_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}

// using Testcontainers.Redis;
// using Xunit;
//
// namespace Airbnb.IntegrationTest.Fixtures;
//
// public class RedisContainerFixture : IAsyncLifetime
// {
//     private RedisContainer Container { get; set; }
//     public string ConnectionString => Container.GetConnectionString();
//
//     public async Task InitializeAsync()
//     {
//         Container = new RedisBuilder()
//             .WithImage("redis:7.2-alpine")
//             .Build();
//         
//         await Container.StartAsync();
//     }
//
//     public async Task DisposeAsync()
//     {
//         await Container.DisposeAsync();
//     }
// }