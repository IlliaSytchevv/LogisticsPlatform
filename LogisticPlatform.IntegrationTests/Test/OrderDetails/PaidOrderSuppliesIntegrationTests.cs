using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Application.DTO.Supplies;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class PaidOrderSuppliesIntegrationTests(LogisticsApiFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task AddSupply_ShouldReturnConflict_WhenOrderAlreadyPaid()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateCrossDockOrderWithSupplyAsync(
            client,
            SeedIds.HubTorontoId,
            SeedIds.CatalogWrap001Id);

        await AuthTestHelper.MarkOrderPaidAsync(fixture.Factory.Services, order.Id, amountCents: 240);

        HttpResponseMessage addResponse = await client.PostAsJsonAsync(
            $"/api/v1/orders/{order.Id}/supplies/from-catalog",
            new AddSupplyFromCatalogRequest(SeedIds.CatalogWrap001Id, Quantity: 1));

        addResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetOrderDetails_ShouldShowPaid_WhenPaymentExists()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateCrossDockOrderWithSupplyAsync(
            client,
            SeedIds.HubTorontoId,
            SeedIds.CatalogWrap001Id);

        await AuthTestHelper.MarkOrderPaidAsync(fixture.Factory.Services, order.Id, amountCents: 240);

        HttpResponseMessage detailsResponse = await client.GetAsync($"/api/v1/orders/{order.Id}");
        detailsResponse.EnsureSuccessStatusCode();

        OrderDetailsResponse? details =
            await detailsResponse.Content.ReadFromJsonAsync<OrderDetailsResponse>(JsonOptions);
        details.ShouldNotBeNull();
        details!.IsPaid.ShouldBeTrue();
    }
}
