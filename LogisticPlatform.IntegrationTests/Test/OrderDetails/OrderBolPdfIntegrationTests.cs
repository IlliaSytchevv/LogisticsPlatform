using System.Net;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.List;

namespace LogisticPlatform.IntegrationTests.Test.OrderDetails;

[Collection(IntegrationCollection.Name)]
public sealed class OrderBolPdfIntegrationTests(LogisticsApiFixture fixture)
{
    [Fact]
    public async Task GetBolPdf_ShouldReturnPdf_WhenOrderExists()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateOrderAsync(client, SeedIds.HubTorontoId);

        HttpResponseMessage response = await client.GetAsync($"/api/v1/orders/{order.Id}/bol.pdf");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/pdf");

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        
        bytes.Length.ShouldBeGreaterThan(100);
    }
}
