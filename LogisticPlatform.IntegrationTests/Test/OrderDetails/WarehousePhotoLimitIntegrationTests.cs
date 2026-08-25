using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using LogisticsPlatform.Domain.DTO.Orders.List;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class WarehousePhotoLimitIntegrationTests(LogisticsApiFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task AddWarehousePhoto_ShouldAcceptUpToMaxPhotos_WhenUnderLimit()
    {
        // Arrange
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        // Act + Assert
        for (int i = 0; i < AddWarehousePhotoCommandValidator.MaxPhotosPerOrder; i++)
        {
            HttpResponseMessage response = await UploadPhotoAsync(client, order.Id, $"dock-{i}.png");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            OrderWarehousePhotoResponse? photo =
                await response.Content.ReadFromJsonAsync<OrderWarehousePhotoResponse>(JsonOptions);
            photo.ShouldNotBeNull();
            photo.FileName.ShouldBe($"dock-{i}.png");
        }
    }

    [Fact]
    public async Task AddWarehousePhoto_ShouldReturnBadRequest_WhenPhotoLimitIsReached()
    {
        // Arrange
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        for (int i = 0; i < AddWarehousePhotoCommandValidator.MaxPhotosPerOrder; i++)
        {
            (await UploadPhotoAsync(client, order.Id, $"fill-{i}.png")).EnsureSuccessStatusCode();
        }

        // Act
        HttpResponseMessage overLimit = await UploadPhotoAsync(client, order.Id, "overflow.png");

        // Assert
        overLimit.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        string body = await overLimit.Content.ReadAsStringAsync();
        body.ShouldContain($"{AddWarehousePhotoCommandValidator.MaxPhotosPerOrder} photos");
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

        return await client.PostAsync($"/api/orders/{orderId}/warehouse-photos", content);
    }
}
