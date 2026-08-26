using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class WarehousePhotoLimitIntegrationTests(LogisticsApiFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task AddWarehousePhoto_ShouldPersistAndReturnPhoto_WhenValidImage()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage response = await UploadPhotoAsync(client, order.Id, "dock-0.png");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        OrderWarehousePhotoResponse? photo =
            await response.Content.ReadFromJsonAsync<OrderWarehousePhotoResponse>(JsonOptions);
        photo.ShouldNotBeNull();
        photo.FileName.ShouldBe("dock-0.png");
    }

    private static async Task<HttpResponseMessage> UploadPhotoAsync(
        HttpClient client,
        Guid orderId,
        string fileName)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(TestImages.TinyPng);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", fileName);

        return await client.PostAsync($"/api/v1/orders/{orderId}/warehouse-photos", content);
    }
}
