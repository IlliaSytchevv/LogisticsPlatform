using System.Net;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class OrderStatusTransitionIntegrationTests(LogisticsApiFixture fixture)
{
    [Fact]
    public async Task PatchOrder_ShouldReturnBadRequest_WhenStatusTransitionInvalid()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        (await AuthTestHelper.AcquireEditLockAsync(client, order.Id)).EnsureSuccessStatusCode();

        HttpResponseMessage patch = await AuthTestHelper.PatchOrderAsync(
            client,
            order.Id,
            new UpdateOrderRequest(
                Number: null,
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
                Status: OrderStatus.Completed,
                AwaitingClientAction: null));

        patch.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
