using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Application.DTO.Payments;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticPlatform.IntegrationTests.Test.Payments;

[Collection(IntegrationCollection.Name)]
public sealed class PaymentsCheckoutIntegrationTests(LogisticsApiFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task CreateCheckout_ShouldReturnCheckoutUrl_WhenCrossDockOrderHasSupplies()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateCrossDockOrderWithSupplyAsync(
            client,
            SeedIds.HubTorontoId,
            SeedIds.CatalogWrap001Id);

        await AuthTestHelper.PromoteDraftToNewAsync(client, order.Id, $"ORD-{Guid.NewGuid():N}"[..12]);

        HttpResponseMessage checkoutResponse =
            await client.PostAsync($"/api/v1/payments/orders/{order.Id}/checkout", content: null);

        checkoutResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        CreateCheckoutResponse? body =
            await checkoutResponse.Content.ReadFromJsonAsync<CreateCheckoutResponse>(JsonOptions);
        body.ShouldNotBeNull();
        body!.CheckoutUrl.ShouldContain("https://checkout.test/");
        body.AmountCents.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task CreateCheckout_ShouldReturnConflict_WhenOrderAlreadyPaid()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateCrossDockOrderWithSupplyAsync(
            client,
            SeedIds.HubTorontoId,
            SeedIds.CatalogWrap001Id);

        await AuthTestHelper.PromoteDraftToNewAsync(client, order.Id, $"ORD-{Guid.NewGuid():N}"[..12]);
        await AuthTestHelper.MarkOrderPaidAsync(fixture.Factory.Services, order.Id, amountCents: 240);

        HttpResponseMessage checkoutResponse =
            await client.PostAsync($"/api/v1/payments/orders/{order.Id}/checkout", content: null);

        checkoutResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task StripeWebhook_ShouldMarkOrderPaid_WhenCheckoutSessionCompleted()
    {
        using HttpClient client = fixture.Factory.CreateClient();
        string token = await AuthTestHelper.LoginAsTestUserAsync(client);
        client.UseBearer(token);

        CreateOrderResponse order = await AuthTestHelper.CreateCrossDockOrderWithSupplyAsync(
            client,
            SeedIds.HubTorontoId,
            SeedIds.CatalogWrap001Id,
            quantity: 2);

        await AuthTestHelper.PromoteDraftToNewAsync(client, order.Id, $"ORD-{Guid.NewGuid():N}"[..12]);

        HttpResponseMessage checkoutResponse = await client.PostAsync($"/api/v1/payments/orders/{order.Id}/checkout", content: null);
        checkoutResponse.EnsureSuccessStatusCode();

        CreateCheckoutResponse? checkout = await checkoutResponse.Content.ReadFromJsonAsync<CreateCheckoutResponse>(JsonOptions);
        checkout.ShouldNotBeNull();

        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        string? sessionId = await db.OrderPayments
            .AsNoTracking()
            .Where(p => p.Id == checkout!.PaymentId)
            .Select(p => p.StripeSessionId)
            .SingleAsync();

        sessionId.ShouldNotBeNullOrWhiteSpace();

        string webhookJson =
            $$"""
              {
                "type": "checkout.session.completed",
                "sessionId": "{{sessionId}}",
                "paymentId": "{{checkout.PaymentId}}",
                "orderId": "{{order.Id}}"
              }
              """;

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook")
        {
            Content = AuthTestHelper.StripeWebhookJson(webhookJson)
        };
        request.Headers.Add("Stripe-Signature", FakeStripeCheckoutService.TestSignature);

        client.DefaultRequestHeaders.Authorization = null;
        HttpResponseMessage webhookResponse = await client.SendAsync(request);
        webhookResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        client.UseBearer(token);

        HttpResponseMessage detailsResponse = await client.GetAsync($"/api/v1/orders/{order.Id}");
        detailsResponse.EnsureSuccessStatusCode();
        OrderDetailsResponse? details =
            await detailsResponse.Content.ReadFromJsonAsync<OrderDetailsResponse>(JsonOptions);
        details.ShouldNotBeNull();
        details!.IsPaid.ShouldBeTrue();
    }
}
