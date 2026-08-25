using Ardalis.Result;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Models.Supplies;
using LogisticsPlatform.Application.UseCases.Orders.CreateOrder;
using LogisticsPlatform.Domain.DTO.Orders.List;
using LogisticsPlatform.Domain.Enums;
using Moq;

namespace LogisticPlatform.UnitTests.Orders;

public sealed class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrdersRepository> _orders = new();
    private readonly Mock<ISupplyCatalogRepository> _catalog = new();
    private readonly Mock<IOrderDetailsRepository> _orderDetails = new();
    private readonly CreateOrderCommandHandler _sut;

    public CreateOrderCommandHandlerTests()
    {
        _sut = new CreateOrderCommandHandler(_orders.Object, _catalog.Object, _orderDetails.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenHubDoesNotExist()
    {
        // Arrange
        var command = CreateCommand();
        _orders.Setup(x => x.HubExistsAsync(command.HubId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        Result<CreateOrderResponse> result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
        _orders.Verify(
            x => x.CreateDraftAsync(
                It.IsAny<OrderType>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var command = CreateCommand();
        _orders.Setup(x => x.HubExistsAsync(command.HubId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orders.Setup(x => x.UserExistsAsync(command.CreatedByUserId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenCatalogItemIsMissing()
    {
        // Arrange
        var catalogItemId = Guid.Parse("d1000000-0000-0000-0000-000000000001");
        var command = CreateCommand(supplies:
        [
            new CreateOrderSupplyLineRequest(catalogItemId, 2)
        ]);
        _orders.Setup(x => x.HubExistsAsync(command.HubId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orders.Setup(x => x.UserExistsAsync(command.CreatedByUserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _catalog
            .Setup(x => x.GetByIdAsync(catalogItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SupplyCatalogItemInternalData?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
        _orders.Verify(
            x => x.CreateDraftAsync(
                It.IsAny<OrderType>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldApplyDestinationDefaults_WhenCityAndRegionAreBlank()
    {
        // Arrange
        var command = CreateCommand(city: "  ", region: null);
        var created = new OrderCreatedData(
            Guid.NewGuid(),
            "FR001999",
            OrderType.Consolidation,
            OrderStatus.Draft);

        _orders.Setup(x => x.HubExistsAsync(command.HubId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orders.Setup(x => x.UserExistsAsync(command.CreatedByUserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orders
            .Setup(x => x.CreateDraftAsync(
                command.Type,
                command.HubId,
                command.CreatedByUserId,
                It.IsAny<DateTimeOffset>(),
                "TBD",
                "ON",
                command.PrimaryReference,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(created.Id);
        result.Value.Number.ShouldBe("FR001999");
        _orders.Verify(
            x => x.CreateDraftAsync(
                command.Type,
                command.HubId,
                command.CreatedByUserId,
                It.IsAny<DateTimeOffset>(),
                "TBD",
                "ON",
                command.PrimaryReference,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAddSuppliesWithPlatformPrice_WhenCatalogLinesProvided()
    {
        // Arrange
        var catalogItemId = Guid.Parse("d1000000-0000-0000-0000-000000000001");
        var command = CreateCommand(
            city: "Toronto",
            region: "ON",
            supplies: [new CreateOrderSupplyLineRequest(catalogItemId, 3)]);

        var catalogItem = new SupplyCatalogItemInternalData(
            catalogItemId,
            "WRAP-001",
            "Shrink wrap 120g",
            "Packaging",
            PlatformPriceCents: 120,
            WholesalePriceCents: 70,
            MarginSplitPercent: 20);

        var created = new OrderCreatedData(
            Guid.Parse("c0000000-0000-0000-0000-000000001999"),
            "FR001999",
            OrderType.CrossDock,
            OrderStatus.Draft);

        _orders.Setup(x => x.HubExistsAsync(command.HubId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orders.Setup(x => x.UserExistsAsync(command.CreatedByUserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _catalog
            .Setup(x => x.GetByIdAsync(catalogItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(catalogItem);
        _orders
            .Setup(x => x.CreateDraftAsync(
                It.IsAny<OrderType>(),
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<DateTimeOffset>(),
                "Toronto",
                "ON",
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);
        _orderDetails
            .Setup(x => x.AddSupplyAsync(
                created.Id,
                catalogItem.Sku,
                catalogItem.Name,
                catalogItem.Category,
                3,
                catalogItem.PlatformPriceCents,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrderSupplyData(
                Guid.NewGuid(),
                created.Id,
                catalogItem.Sku,
                catalogItem.Name,
                catalogItem.Category,
                3,
                catalogItem.PlatformPriceCents,
                360));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _orderDetails.Verify(
            x => x.AddSupplyAsync(
                created.Id,
                "WRAP-001",
                "Shrink wrap 120g",
                "Packaging",
                3,
                120,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _orderDetails.Verify(
            x => x.AddSupplyAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                70,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static CreateOrderCommand CreateCommand(
        string? city = "Toronto",
        string? region = "ON",
        IReadOnlyList<CreateOrderSupplyLineRequest>? supplies = null) =>
        new(
            OrderType.Consolidation,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ScheduledAt: null,
            DestinationCity: city,
            DestinationRegion: region,
            PrimaryReference: "REF-1",
            Supplies: supplies);
}