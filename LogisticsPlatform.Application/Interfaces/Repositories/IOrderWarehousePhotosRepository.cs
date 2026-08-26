using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderWarehousePhotosRepository
{
    Task<OrderWarehousePhotoData> AddWarehousePhotoAsync(
        Guid orderId,
        Guid photoId,
        string fileName,
        string contentType,
        string storageKey,
        CancellationToken cancellationToken);

    Task<OrderWarehousePhotoContentData?> GetWarehousePhotoContentAsync(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken);

    Task<string?> SoftDeleteWarehousePhotoAsync(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken);
}
