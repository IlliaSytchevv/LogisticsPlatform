using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderOperationsRepository
{
    Task<OrderOperationData> AddOperationAsync(
        Guid orderId,
        OrderOperationType type,
        string? trailer,
        int quantity,
        PalletUnit unit,
        string? unitLabel,
        DateTimeOffset appliedAt,
        CancellationToken cancellationToken);

    Task<bool> OperationExistsAsync(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<string>?> SoftDeleteOperationAsync(
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
        Guid photoId,
        string fileName,
        string contentType,
        string storageKey,
        CancellationToken cancellationToken);

    Task<OrderOperationPhotoContentData?> GetOperationPhotoContentAsync(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken);

    Task<string?> SoftDeleteOperationPhotoAsync(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken);
}
