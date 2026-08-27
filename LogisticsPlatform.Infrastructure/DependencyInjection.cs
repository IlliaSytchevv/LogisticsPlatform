using System.Text;
using LogisticsPlatform.Application;
using LogisticsPlatform.Application.Interfaces.FileExport;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Options;
using LogisticsPlatform.Infrastructure.Database;
using LogisticsPlatform.Infrastructure.FileExport;
using LogisticsPlatform.Infrastructure.Redis;
using LogisticsPlatform.Infrastructure.Repositories;
using LogisticsPlatform.Infrastructure.Repositories.OrderDetails;
using LogisticsPlatform.Infrastructure.RepositoriesDecorator;
using LogisticsPlatform.Infrastructure.Services;
using LogisticsPlatform.Infrastructure.Stripe;
using LogisticsPlatform.Infrastructure.Wrappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace LogisticsPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.Configure<PhotoStorageOptions>(configuration.GetSection(PhotoStorageOptions.SectionName));
        services.Configure<StripeOptions>(configuration.GetSection(StripeOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            string connectionString = sp.GetRequiredService<IOptions<RedisOptions>>().Value.ConnectionString
                ?? throw new InvalidOperationException("Redis:ConnectionString is missing.");
            var options = ConfigurationOptions.Parse(connectionString, ignoreUnknown: true);
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });
        services.AddSingleton<RedLockFactory>(sp =>
        {
            var multiplexer = (ConnectionMultiplexer)sp.GetRequiredService<IConnectionMultiplexer>();
            var multiplexers = new List<RedLockMultiplexer> { new(multiplexer) };
            return RedLockFactory.Create(multiplexers);
        });
        services.AddSingleton<IRedisService, RedisService>();
        services.AddSingleton<IRedisLock, RedisLock>();
        services.AddSingleton<IOrderEditLock, OrderEditLock>();
        services.AddSingleton<IOrderCheckoutLock, OrderCheckoutLock>();
        services.AddSingleton<INotificationsFeedCacheInvalidator, NotificationsFeedCacheInvalidator>();

        services.AddSingleton<IPhotoBlobStore, AzureBlobPhotoStore>();
        services.AddScoped<IUserManagerWrapper, UserManagerWrapper>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<NotificationsRepository>();
        services.AddScoped<INotificationsRepository>(sp =>
            new CachedNotificationsRepositoryDecorator(
                sp.GetRequiredService<NotificationsRepository>(),
                sp.GetRequiredService<IRedisService>(),
                sp.GetRequiredService<ILogger<CachedNotificationsRepositoryDecorator>>()));
        services.AddScoped<ISupplyCatalogRepository, SupplyCatalogRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IOrderAccessRepository, OrderAccessRepository>();
        services.AddScoped<IOrderDetailsQueryRepository, OrderDetailsQueryRepository>();
        services.AddScoped<IOrderDocumentsQueryRepository, OrderDocumentsQueryRepository>();
        services.AddScoped<IOrderPatchRepository, OrderPatchRepository>();
        services.AddScoped<IOrderOperationsRepository, OrderOperationsRepository>();
        services.AddScoped<IOrderSuppliesRepository, OrderSuppliesRepository>();
        services.AddScoped<IOrderWarehousePhotosRepository, OrderWarehousePhotosRepository>();
        services.AddScoped<IOrderCommentsRepository, OrderCommentsRepository>();
        services.AddScoped<IOrderTimelineRepository, OrderTimelineRepository>();
        services.AddScoped<IOrderPaymentsRepository, OrderPaymentsRepository>();
        services.AddScoped<IStripeCheckoutService, StripeCheckoutService>();
        services.AddScoped<IFileWriter, CsvExportWriter>();
        services.AddScoped<IOrdersExportSource, OrdersExportBatchReader>();
        services.AddScoped<IOrderBolPdfService, OrderBolPdfService>();
        services.AddScoped<IOrderQrService, OrderQrService>();

        JwtOptions jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    NameClaimType = System.Security.Claims.ClaimTypes.Name,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        string? typ = context.Principal?.FindFirst(JwtTokenService.TokenTypeClaim)?.Value;
                        if (string.Equals(typ, JwtTokenService.RefreshTokenType, StringComparison.Ordinal))
                        {
                            context.Fail("Refresh token cannot be used as access token.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
