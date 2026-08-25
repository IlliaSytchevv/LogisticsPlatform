using Ardalis.Result;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;
using LogisticsPlatform.Domain.DTO.Orders.Detail;
using Moq;

namespace LogisticPlatform.UnitTests.OrderDetails;

public sealed class AddWarehousePhotoCommandHandlerTests
{
    private readonly Mock<IOrderDetailsRepository> _orderDetails = new();
    private readonly AddWarehousePhotoCommandHandler _sut;

    public AddWarehousePhotoCommandHandlerTests()
    {
        _sut = new AddWarehousePhotoCommandHandler(_orderDetails.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        _orderDetails.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        Result<OrderWarehousePhotoResponse> result = await _sut.Handle(
            CreateCommand(orderId),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        _orderDetails.Verify(
            x => x.AddWarehousePhotoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalid_WhenPhotoLimitIsReached()
    {
        var orderId = Guid.NewGuid();
        _orderDetails.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orderDetails
            .Setup(x => x.CountWarehousePhotosAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AddWarehousePhotoCommandValidator.MaxPhotosPerOrder);

        var result = await _sut.Handle(CreateCommand(orderId), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e =>
            e.ErrorMessage.Contains($"{AddWarehousePhotoCommandValidator.MaxPhotosPerOrder} photos"));
        _orderDetails.Verify(
            x => x.AddWarehousePhotoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseExistingCountAsSortOrder_WhenSortOrderIsNull()
    {
        var orderId = Guid.NewGuid();
        const int existingCount = 2;
        var saved = new OrderWarehousePhotoData(
            Guid.NewGuid(),
            orderId,
            "dock.jpg",
            "image/jpeg",
            existingCount);

        _orderDetails.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orderDetails
            .Setup(x => x.CountWarehousePhotosAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCount);
        _orderDetails
            .Setup(x => x.AddWarehousePhotoAsync(
                orderId,
                "dock.jpg",
                "image/jpeg",
                It.IsAny<byte[]>(),
                existingCount,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(saved);

        var result = await _sut.Handle(
            CreateCommand(orderId, sortOrder: null),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.SortOrder.ShouldBe(existingCount);
        _orderDetails.Verify(
            x => x.AddWarehousePhotoAsync(
                orderId,
                "dock.jpg",
                "image/jpeg",
                It.IsAny<byte[]>(),
                existingCount,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUseProvidedSortOrder_WhenSortOrderIsSet()
    {
        var orderId = Guid.NewGuid();
        const int sortOrder = 9;
        var saved = new OrderWarehousePhotoData(
            Guid.NewGuid(),
            orderId,
            "dock.jpg",
            "image/jpeg",
            sortOrder);

        _orderDetails.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _orderDetails
            .Setup(x => x.CountWarehousePhotosAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _orderDetails
            .Setup(x => x.AddWarehousePhotoAsync(
                orderId,
                "dock.jpg",
                "image/jpeg",
                It.IsAny<byte[]>(),
                sortOrder,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(saved);

        var result = await _sut.Handle(
            CreateCommand(orderId, sortOrder: sortOrder),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.SortOrder.ShouldBe(sortOrder);
        _orderDetails.Verify(
            x => x.AddWarehousePhotoAsync(
                orderId,
                "dock.jpg",
                "image/jpeg",
                It.IsAny<byte[]>(),
                sortOrder,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static AddWarehousePhotoCommand CreateCommand(Guid orderId, int? sortOrder = null) =>
        new(
            orderId,
            "dock.jpg",
            "image/jpeg",
            Content: [1, 2, 3],
            SortOrder: sortOrder);
}