using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;

namespace LogisticPlatform.IntegrationTests.Test.Orders;

[Collection(IntegrationCollection.Name)]
public sealed class CreateOrderIntegrationTests(LogisticsApiFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task CreateOrder_ShouldReturnOkAndPersistOrder_WhenAuthorized()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        HttpResponseMessage createResponse = await client.PostAsJsonAsync(
            "/api/v1/orders",
            new CreateOrderRequest(
                OrderType.Consolidation,
                SeedIds.HubTorontoId,
                ScheduledAt: DateTimeOffset.UtcNow.AddDays(2),
                DestinationCity: "Toronto",
                DestinationRegion: "ON",
                PrimaryReference: $"IT-{Guid.NewGuid():N}"[..16],
                Supplies: null));

        createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        CreateOrderResponse? created =
            await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>(JsonOptions);
        created.ShouldNotBeNull();
        created!.Id.ShouldNotBe(Guid.Empty);
        created.Number.ShouldNotBeNullOrWhiteSpace();
        created.Status.ShouldBe(OrderStatus.Draft);

        HttpResponseMessage detailsResponse = await client.GetAsync($"/api/v1/orders/{created.Id}");
        detailsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        OrderDetailsResponse? details =
            await detailsResponse.Content.ReadFromJsonAsync<OrderDetailsResponse>(JsonOptions);
        details.ShouldNotBeNull();
        details!.Id.ShouldBe(created.Id);
        details.Number.ShouldBe(created.Number);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        using HttpClient client = fixture.Factory.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/orders",
            new CreateOrderRequest(
                OrderType.Consolidation,
                SeedIds.HubTorontoId,
                ScheduledAt: DateTimeOffset.UtcNow.AddDays(1),
                DestinationCity: "Toronto",
                DestinationRegion: "ON",
                PrimaryReference: "NO-AUTH",
                Supplies: null));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
