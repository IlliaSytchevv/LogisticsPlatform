using Ardalis.Result;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;
using Moq;

namespace LogisticPlatform.UnitTests.OrderDetails;

public sealed class AddWarehousePhotoCommandHandlerTests
{
    private readonly Mock<IOrderAccessRepository> _orderAccess = new();
    private readonly Mock<IOrderWarehousePhotosRepository> _warehousePhotos = new();
    private readonly Mock<IPhotoBlobStore> _photoBlobStore = new();
    private readonly AddWarehousePhotoCommandHandler _sut;

    public AddWarehousePhotoCommandHandlerTests()
    {
        _sut = new AddWarehousePhotoCommandHandler(
            _orderAccess.Object,
            _warehousePhotos.Object,
            _photoBlobStore.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        _orderAccess.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        Result<OrderWarehousePhotoResponse> result = await _sut.Handle(
            CreateCommand(orderId),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        _warehousePhotos.Verify(
            x => x.AddWarehousePhotoAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSaveBlobAndPersist_WhenOrderExists()
    {
        var orderId = Guid.NewGuid();
        var saved = new OrderWarehousePhotoData(
            Guid.NewGuid(),
            orderId,
            "dock.jpg",
            "image/jpeg");

        _orderAccess.Setup(x => x.ExistsAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _warehousePhotos
            .Setup(x => x.AddWarehousePhotoAsync(
                orderId,
                It.IsAny<Guid>(),
                "dock.jpg",
                "image/jpeg",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(saved);

        var result = await _sut.Handle(CreateCommand(orderId), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.FileName.ShouldBe("dock.jpg");
        _photoBlobStore.Verify(
            x => x.SaveAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _warehousePhotos.Verify(
            x => x.AddWarehousePhotoAsync(
                orderId,
                It.IsAny<Guid>(),
                "dock.jpg",
                "image/jpeg",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static AddWarehousePhotoCommand CreateCommand(Guid orderId) =>
        new(
            orderId,
            "dock.jpg",
            "image/jpeg",
            Content:
            [
                0xFF, 0xD8, 0xFF, 0xE0,
                0x00, 0x10, 0x4A, 0x46, 0x49, 0x46
            ]);
}
