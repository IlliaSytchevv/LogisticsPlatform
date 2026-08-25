using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using LogisticsPlatform.Domain.DTO.Orders.List;
using LogisticsPlatform.Domain.DTO.Supplies;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class SoftDeleteSupplyIntegrationTests(LogisticsApiFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task DeleteSupply_ShouldHideSupplyFromDetailsAndMarkDeleted_WhenDeleted()
    {
        // Arrange
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage addResponse = await client.PostAsJsonAsync(
            $"/api/orders/{order.Id}/supplies/from-catalog",
            new AddSupplyFromCatalogRequest(SeedIds.CatalogWrap001Id, Quantity: 2));
        addResponse.EnsureSuccessStatusCode();

        OrderSupplyResponse? supply =
            await addResponse.Content.ReadFromJsonAsync<OrderSupplyResponse>(JsonOptions);
        supply.ShouldNotBeNull();

        // Act
        HttpResponseMessage deleteResponse =
            await client.DeleteAsync($"/api/orders/{order.Id}/supplies/{supply.Id}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        HttpResponseMessage detailsResponse = await client.GetAsync($"/api/orders/{order.Id}");
        detailsResponse.EnsureSuccessStatusCode();
        OrderDetailsResponse? details =
            await detailsResponse.Content.ReadFromJsonAsync<OrderDetailsResponse>(JsonOptions);
        details.ShouldNotBeNull();
        details.Supplies.ShouldNotContain(s => s.Id == supply.Id);

        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var row = await db.OrderSupplies
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleAsync(x => x.Id == supply.Id);

        row.IsDeleted.ShouldBeTrue();
        row.DeletedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteSupply_ShouldReturnNotFound_WhenSupplyAlreadyDeleted()
    {
        // Arrange
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage addResponse = await client.PostAsJsonAsync(
            $"/api/orders/{order.Id}/supplies/from-catalog",
            new AddSupplyFromCatalogRequest(SeedIds.CatalogWrap001Id, Quantity: 1));
        addResponse.EnsureSuccessStatusCode();
        OrderSupplyResponse? supply =
            await addResponse.Content.ReadFromJsonAsync<OrderSupplyResponse>(JsonOptions);
        supply.ShouldNotBeNull();

        (await client.DeleteAsync($"/api/orders/{order.Id}/supplies/{supply.Id}"))
            .EnsureSuccessStatusCode();

        // Act
        HttpResponseMessage secondDelete =
            await client.DeleteAsync($"/api/orders/{order.Id}/supplies/{supply.Id}");

        // Assert
        secondDelete.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
