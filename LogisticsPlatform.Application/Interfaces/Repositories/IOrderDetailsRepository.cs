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

    Task<OrderSupplyData?> UpdateSupplyAsync(
        Guid orderId,
        Guid supplyId,
        string sku,
        string name,
        string category,
        int quantity,
        long unitPriceCents,
        CancellationToken cancellationToken);

    Task<OrderSupplyData?> UpdateSupplyQuantityAsync(
        Guid orderId,
        Guid supplyId,
        int quantity,
        CancellationToken cancellationToken);

    Task<bool> SoftDeleteSupplyAsync(Guid orderId, Guid supplyId, CancellationToken cancellationToken);

    Task<int> CountWarehousePhotosAsync(Guid orderId, CancellationToken cancellationToken);

    Task<OrderWarehousePhotoData> AddWarehousePhotoAsync(
        Guid orderId,
        string fileName,
        string contentType,
        byte[] content,
        int sortOrder,
        CancellationToken cancellationToken);

    Task<OrderWarehousePhotoContentData?> GetWarehousePhotoContentAsync(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken);

    Task<bool> SoftDeleteWarehousePhotoAsync(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderCommentData>> GetCommentsAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<OrderCommentData> AddCommentAsync(
        Guid orderId,
        string text,
        string? authorName,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderTimelineEntryData>> GetTimelineAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<OrderTimelineEntryData> AddTimelineEntryAsync(
        Guid orderId,
        string kind,
        string text,
        string? authorName,
        CancellationToken cancellationToken);

    Task<bool> OperationExistsAsync(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderOperationCommentData>> GetOperationCommentsAsync(
        Guid operationId,
        CancellationToken cancellationToken);

    Task<OrderOperationCommentData> AddOperationCommentAsync(
        Guid operationId,
        string text,
        string? authorName,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderOperationPhotoData>> GetOperationPhotosAsync(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken);

    Task<OrderOperationPhotoData> AddOperationPhotoAsync(
        Guid orderId,
        Guid operationId,
        string fileName,
        string contentType,
        byte[] content,
        int sortOrder,
        CancellationToken cancellationToken);

    Task<OrderOperationPhotoContentData?> GetOperationPhotoContentAsync(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken);

    Task<bool> SoftDeleteOperationPhotoAsync(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken);
}
