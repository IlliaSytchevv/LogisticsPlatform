using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace LogisticPlatform.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _postgresConnection;
    private readonly string _redisConnection;

    public CustomWebApplicationFactory(string postgresConnection, string redisConnection)
    {
        _postgresConnection = postgresConnection;
        _redisConnection = redisConnection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        string redisConnection = EnsureAbortConnectFalse(_redisConnection);

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgresConnection,
                ["Redis:ConnectionString"] = redisConnection,
                ["Stripe:SecretKey"] = "sk_test_integration_fake",
                ["Stripe:WebhookSecret"] = "whsec_integration_test",
                ["Stripe:SuccessUrlTemplate"] = "http://localhost/orders/{orderId}?payment=success",
                ["Stripe:CancelUrlTemplate"] = "http://localhost/orders/{orderId}?payment=cancel",
                ["PhotoStorage:ConnectionString"] = "UseDevelopmentStorage=true",
                ["PhotoStorage:ContainerName"] = "photos-test",
            });
        });

        builder.ConfigureServices(services =>
        {
            RemoveDbContextRegistrations(services);
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_postgresConnection));

            services.RemoveAll<IConnectionMultiplexer>();
            services.RemoveAll<RedLockFactory>();
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
            services.AddSingleton<RedLockFactory>(sp =>
            {
                var multiplexer = (ConnectionMultiplexer)sp.GetRequiredService<IConnectionMultiplexer>();
                return RedLockFactory.Create(new List<RedLockMultiplexer> { new(multiplexer) });
            });

            services.RemoveAll<IStripeCheckoutService>();
            services.AddScoped<IStripeCheckoutService, FakeStripeCheckoutService>();

            services.RemoveAll<IPhotoBlobStore>();
            services.AddSingleton<IPhotoBlobStore, InMemoryPhotoBlobStore>();
        });
    }

    private static string EnsureAbortConnectFalse(string connectionString)
    {
        if (connectionString.Contains("abortConnect=", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        return $"{connectionString},abortConnect=false";
    }

    private static void RemoveDbContextRegistrations(IServiceCollection services)
    {
        ServiceDescriptor[] toRemove = services
            .Where(descriptor =>
                descriptor.ServiceType == typeof(AppDbContext)
                || descriptor.ServiceType == typeof(DbContextOptions)
                || descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>))
            .ToArray();

        foreach (ServiceDescriptor descriptor in toRemove)
        {
            services.Remove(descriptor);
        }
    }
}