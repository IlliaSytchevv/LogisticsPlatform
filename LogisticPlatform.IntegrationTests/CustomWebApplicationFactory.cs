using LogisticsPlatform.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticPlatform.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _postgresConnection;

    public CustomWebApplicationFactory(string postgresConnection)
    {
        _postgresConnection = postgresConnection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            RemoveDbContextRegistrations(services);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_postgresConnection));
        });
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