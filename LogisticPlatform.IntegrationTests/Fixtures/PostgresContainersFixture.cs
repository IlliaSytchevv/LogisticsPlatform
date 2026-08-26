using Testcontainers.PostgreSql;

namespace LogisticPlatform.IntegrationTests.Fixtures;

public sealed class PostgresContainersFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;

    public string ConnectionString =>
        _container.GetConnectionString()
        ?? throw new InvalidOperationException("PostgreSQL test container connection string is unavailable.");

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
        await _container.DisposeAsync();
    }
}
