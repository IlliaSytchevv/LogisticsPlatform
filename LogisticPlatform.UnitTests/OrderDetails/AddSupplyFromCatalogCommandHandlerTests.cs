using Ardalis.Result;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Models.Supplies;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddSupplyFromCatalog;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using Moq;

namespace LogisticPlatform.UnitTests.OrderDetails;

public sealed class AddSupplyFromCatalogCommandHandlerTests
{
    private readonly Mock<IOrderDetailsRepository> _orderDetails = new();
    private readonly Mock<ISupplyCatalogRepository> _catalog = new();
    private readonly AddSupplyFromCatalogCommandHandler _sut;

    public AddSupplyFromCatalogCommandHandlerTests()
    {
        _sut = new AddSupplyFromCatalogCommandHandler(_orderDetails.Object, _catalog.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        _orderDetails.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        Result<OrderSupplyResponse> result = await _sut.Handle(
            new AddSupplyFromCatalogCommand(orderId, Guid.NewGuid(), 1),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        _catalog.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenCatalogItemDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        var catalogItemId = Guid.NewGuid();
        
        _orderDetails.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _catalog
            .Setup(x => x.GetByIdAsync(catalogItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SupplyCatalogItemInternalData?)null);

        var result = await _sut.Handle(
            new AddSupplyFromCatalogCommand(orderId, catalogItemId, 2),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        _orderDetails.Verify(
            x => x.AddSupplyAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPersistPlatformPriceOnly_WhenCatalogItemExists()
    {
        var orderId = Guid.Parse("c0000000-0000-0000-0000-000000001676");
        var catalogItemId = Guid.Parse("d1000000-0000-0000-0000-000000000001");
        const long platformPrice = 120;
        const long wholesalePrice = 70;

        var catalogItem = new SupplyCatalogItemInternalData(
            catalogItemId,
            "WRAP-001",
            "Shrink wrap 120g",
            "Packaging",
            platformPrice,
            wholesalePrice,
            MarginSplitPercent: 20);

        var saved = new OrderSupplyData(
            Guid.NewGuid(),
            orderId,
            catalogItem.Sku,
            catalogItem.Name,
            catalogItem.Category,
            Quantity: 4,
            UnitPriceCents: platformPrice,
            LineTotalCents: platformPrice * 4);

        _orderDetails.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _catalog
            .Setup(x => x.GetByIdAsync(catalogItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(catalogItem);
        _orderDetails
            .Setup(x => x.AddSupplyAsync(
                orderId,
                catalogItem.Sku,
                catalogItem.Name,
                catalogItem.Category,
                4,
                platformPrice,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(saved);

        var result = await _sut.Handle(
            new AddSupplyFromCatalogCommand(orderId, catalogItemId, 4),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.UnitPriceCents.ShouldBe(platformPrice);
        result.Value.Sku.ShouldBe("WRAP-001");
        _orderDetails.Verify(x => x.AddSupplyAsync(orderId, "WRAP-001", "Shrink wrap 120g", "Packaging", 
                4, platformPrice, It.IsAny<CancellationToken>()),
            Times.Once);
        _orderDetails.Verify(x => x.AddSupplyAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int>(), wholesalePrice, It.IsAny<CancellationToken>()),
            Times.Never);
    }
}