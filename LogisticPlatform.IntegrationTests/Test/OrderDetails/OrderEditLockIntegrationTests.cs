using System.Net;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class OrderEditLockIntegrationTests(LogisticsApiFixture fixture)
{
    [Fact]
    public async Task PatchOrder_ShouldReturnConflict_WhenEditLockNotHeld()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage patch = await AuthTestHelper.PatchOrderAsync(
            client,
            order.Id,
            new UpdateOrderRequest(
                Number: null,
                CustomerName: "Locked test",
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
                Status: null,
                AwaitingClientAction: null));

        patch.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PatchOrder_ShouldSucceed_WhenEditLockHeld()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage acquire = await AuthTestHelper.AcquireEditLockAsync(client, order.Id);
        acquire.StatusCode.ShouldBe(HttpStatusCode.OK);

        HttpResponseMessage patch = await AuthTestHelper.PatchOrderAsync(
            client,
            order.Id,
            new UpdateOrderRequest(
                Number: null,
                CustomerName: "After lock",
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
                Status: null,
                AwaitingClientAction: null));

        patch.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AcquireEditLock_ShouldReturnConflict_WhenAnotherUserAlreadyEditing()
    {
        using HttpClient first = fixture.Factory.CreateClient();
        using HttpClient second = fixture.Factory.CreateClient();

        string firstToken = await AuthTestHelper.LoginAsTestUserAsync(first);
        string secondToken = await AuthTestHelper.LoginAsDispatcherAsync(second);
        first.UseBearer(firstToken);
        second.UseBearer(secondToken);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(first, SeedIds.HubTorontoId);

        (await AuthTestHelper.AcquireEditLockAsync(first, order.Id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await AuthTestHelper.AcquireEditLockAsync(second, order.Id)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
