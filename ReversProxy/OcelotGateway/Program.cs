using AspNetCoreRateLimit;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

var redisConnection =
    builder.Configuration["Redis:ConnectionString"]
    ?? "localhost:6379,abortConnect=false";

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services
    .AddOcelot(builder.Configuration)
    .AddPolly();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();
    await ipPolicyStore.SeedAsync();
}

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("OcelotGateway.Request");

    logger.LogInformation(
        "IN {Method} {Path}{Query}",
        context.Request.Method,
        context.Request.Path,
        context.Request.QueryString);

    await next();

    logger.LogInformation(
        "OUT {StatusCode} {Method} {Path}",
        context.Response.StatusCode,
        context.Request.Method,
        context.Request.Path);
});

app.UseIpRateLimiting();

await app.UseOcelot();

app.Run();