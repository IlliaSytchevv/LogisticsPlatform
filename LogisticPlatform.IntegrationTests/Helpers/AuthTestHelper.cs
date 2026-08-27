using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using LogisticsPlatform.Application.DTO.Authorization;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticPlatform.IntegrationTests.Helpers;

public static class AuthTestHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<string> LoginAsync(HttpClient client, string username, string password)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(username, password));

        response.EnsureSuccessStatusCode();

        AuthTokenResponse? body = await response.Content.ReadFromJsonAsync<AuthTokenResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.JwtToken.ShouldNotBeNullOrWhiteSpace();
        return body.JwtToken;
    }

    public static Task<string> LoginAsTestUserAsync(HttpClient client) =>
        LoginAsync(client, "AdminUser", "Test123!");

    public static Task<string> LoginAsDispatcherAsync(HttpClient client) =>
        LoginAsync(client, "DispatcherUser", "Test123!");

    public static void UseBearer(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public static Task<CreateOrderResponse> CreateCrossDockOrderWithSupplyAsync(
        HttpClient client,
        Guid hubId,
        Guid catalogItemId,
        int quantity = 2) =>
        CreateOrderAsync(
            client,
            hubId,
            supplies:
            [
                new CreateOrderSupplyLineRequest(catalogItemId, quantity)
            ],
            type: OrderType.CrossDock);

    public static async Task<CreateOrderResponse> CreateOrderAsync(
        HttpClient client,
        Guid hubId,
        IReadOnlyList<CreateOrderSupplyLineRequest>? supplies = null,
        OrderType type = OrderType.Consolidation)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/orders",
            new CreateOrderRequest(
                type,
                hubId,
                ScheduledAt: DateTimeOffset.UtcNow.AddDays(1),
                DestinationCity: "Toronto",
                DestinationRegion: "ON",
                PrimaryReference: $"IT-{Guid.NewGuid():N}"[..16],
                Supplies: supplies));

        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"CreateOrder failed: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        CreateOrderResponse? created =
            await response.Content.ReadFromJsonAsync<CreateOrderResponse>(JsonOptions);

        created.ShouldNotBeNull();
        return created!;
    }

    public static async Task<HttpResponseMessage> AcquireEditLockAsync(HttpClient client, Guid orderId) =>
        await client.PostAsync($"/api/v1/orders/{orderId}/edit-lock", content: null);

    public static Task<HttpResponseMessage> PatchOrderAsync(
        HttpClient client,
        Guid orderId,
        UpdateOrderRequest request) =>
        client.PatchAsJsonAsync($"/api/v1/orders/{orderId}", request);

    public static async Task PromoteDraftToNewAsync(HttpClient client, Guid orderId, string orderNumber)
    {
        (await AcquireEditLockAsync(client, orderId)).EnsureSuccessStatusCode();
        HttpResponseMessage patch = await PatchOrderAsync(
            client,
            orderId,
            new UpdateOrderRequest(
                Number: orderNumber,
                CustomerName: null,
                PrimaryReference: null,
                DeclaredQty: null,
                ActualQty: null,
                TrailerType: null,
                Phone: null,
                TruckNumber: null,
                TrailerNumber: null,
                DockCode: null,
                DockBay: null,
                WarehouseNote: null,
                StockStatusLabel: null,
                LoadingStatusLabel: null,
                Status: OrderStatus.New,
                AwaitingClientAction: null));
        patch.EnsureSuccessStatusCode();
        await client.DeleteAsync($"/api/v1/orders/{orderId}/release-edit-lock");
    }

    public static async Task MarkOrderPaidAsync(
        IServiceProvider services,
        Guid orderId,
        long amountCents)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.OrderPayments.Add(new OrderPayment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            AmountCents = amountCents,
            Currency = "usd",
            Status = OrderPaymentStatus.Paid,
            CreatedAt = DateTimeOffset.UtcNow,
            PaidAt = DateTimeOffset.UtcNow,
            StripeSessionId = $"cs_test_paid_{Guid.NewGuid():N}"
        });

        await db.SaveChangesAsync();
    }

    public static StringContent StripeWebhookJson(string json) =>
        new(json, Encoding.UTF8, "application/json");
}

public static class SeedIds
{
    public static readonly Guid HubTorontoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
    public static readonly Guid CatalogWrap001Id = Guid.Parse("d1000000-0000-0000-0000-000000000001");
    public static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
}

public static class TestImages
{
    public static readonly byte[] TinyPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");
}
