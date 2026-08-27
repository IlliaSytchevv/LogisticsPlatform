using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class WarehousePhotoBlobIntegrationTests(LogisticsApiFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task AddWarehousePhoto_ShouldPersistInBlobStore_AndServeExactBytes()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage upload = await UploadWarehousePhotoAsync(client, order.Id, "blob-dock.png");
        upload.StatusCode.ShouldBe(HttpStatusCode.OK);

        OrderWarehousePhotoResponse? photo = await upload.Content.ReadFromJsonAsync<OrderWarehousePhotoResponse>(JsonOptions);
        photo.ShouldNotBeNull();

        HttpResponseMessage download = await client.GetAsync(photo.DownloadUrl);
        download.StatusCode.ShouldBe(HttpStatusCode.OK);

        byte[] bytes = await download.Content.ReadAsByteArrayAsync();
        
        bytes.ShouldBe(TestImages.TinyPng);
        download.Content.Headers.ContentType?.MediaType.ShouldBe("image/png");
    }

    [Fact]
    public async Task AddWarehousePhoto_ShouldBeReadableFromIPhotoBlobStore()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage upload = await UploadWarehousePhotoAsync(client, order.Id, "store-read.png");
        upload.StatusCode.ShouldBe(HttpStatusCode.OK);

        OrderWarehousePhotoResponse? photo = await upload.Content.ReadFromJsonAsync<OrderWarehousePhotoResponse>(JsonOptions);
        photo.ShouldNotBeNull();

        string storageKey = PhotoStorageKeys.ForWarehouse(order.Id, photo.Id, "image/png");

        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        IPhotoBlobStore blobStore = scope.ServiceProvider.GetRequiredService<IPhotoBlobStore>();

        await using Stream? stream = await blobStore.OpenReadAsync(storageKey, CancellationToken.None);
        stream.ShouldNotBeNull();

        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        
        memory.ToArray().ShouldBe(TestImages.TinyPng);
    }

    [Fact]
    public async Task DeleteWarehousePhoto_ShouldRemoveBlob_AndReturnNotFoundOnDownload()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage upload = await UploadWarehousePhotoAsync(client, order.Id, "to-delete.png");
        upload.StatusCode.ShouldBe(HttpStatusCode.OK);

        OrderWarehousePhotoResponse? photo = await upload.Content.ReadFromJsonAsync<OrderWarehousePhotoResponse>(JsonOptions);
        photo.ShouldNotBeNull();

        HttpResponseMessage delete = await client.DeleteAsync($"/api/v1/orders/{order.Id}/warehouse-photos/{photo.Id}");
        delete.StatusCode.ShouldBe(HttpStatusCode.OK);

        HttpResponseMessage download = await client.GetAsync(photo.DownloadUrl);
        download.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        string storageKey = PhotoStorageKeys.ForWarehouse(order.Id, photo.Id, "image/png");
        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        IPhotoBlobStore blobStore = scope.ServiceProvider.GetRequiredService<IPhotoBlobStore>();

        await using Stream? stream = await blobStore.OpenReadAsync(storageKey, CancellationToken.None);
        
        stream.ShouldBeNull();
    }

    private static async Task<HttpResponseMessage> UploadWarehousePhotoAsync(
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