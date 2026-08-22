using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderDetailsRepository
{
    Task<bool> ExistsAsync(Guid orderId, CancellationToken cancellationToken);

    Task<OrderDetailsData?> GetDetailsAsync(Guid orderId, CancellationToken cancellationToken);

    Task<OrderDocumentData?> GetDocumentDataAsync(Guid orderId, CancellationToken cancellationToken);

    Task<bool> PatchOrderAsync(OrderDetailPatchData patch, CancellationToken cancellationToken);

    Task<OrderOperationData> AddOperationAsync(
        Guid orderId,
        OrderOperationType type,
        string? trailer,
        int quantity,
        PalletUnit unit,
        string? unitLabel,
        DateTimeOffset appliedAt,
        CancellationToken cancellationToken);

    Task<bool> SoftDeleteOperationAsync(Guid orderId, Guid operationId, CancellationToken cancellationToken);

    Task<OrderSupplyData> AddSupplyAsync(
        Guid orderId,
        string sku,
        string name,
        string category,
        int quantity,
        long unitPriceCents,
        CancellationToken cancellationToken);

    Task<bool> SoftDeleteSupplyAsync(Guid orderId, Guid supplyId, CancellationToken cancellationToken);

    Task<OrderWarehousePhotoData> AddWarehousePhotoAsync(
        Guid orderId,
        string url,
        int sortOrder,
        CancellationToken cancellationToken);

    Task<bool> SoftDeleteWarehousePhotoAsync(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken);
}
