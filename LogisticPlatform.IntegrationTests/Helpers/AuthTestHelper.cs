using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticsPlatform.Domain.DTO.Authorization;
using LogisticsPlatform.Domain.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;

namespace LogisticPlatform.IntegrationTests.Helpers;

public static class AuthTestHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<string> LoginAsTestUserAsync(HttpClient client)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("testuser", "Test123!"));

        response.EnsureSuccessStatusCode();

        AuthTokenResponse? body = await response.Content.ReadFromJsonAsync<AuthTokenResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body.JwtToken.ShouldNotBeNullOrWhiteSpace();
        return body.JwtToken;
    }

    public static void UseBearer(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public static async Task<CreateOrderResponse> CreateOrderAsync(
        HttpClient client,
        Guid hubId,
        IReadOnlyList<CreateOrderSupplyLineRequest>? supplies = null)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(
                OrderType.Consolidation,
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