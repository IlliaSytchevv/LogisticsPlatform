using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderOperationsRepository(AppDbContext dbContext) : IOrderOperationsRepository
{
    public async Task<OrderOperationData> AddOperationAsync(
        Guid orderId,
        OrderOperationType type,
        string? trailer,
        int quantity,
        PalletUnit unit,
        string? unitLabel,
        DateTimeOffset appliedAt,
        CancellationToken cancellationToken)
    {
        var entity = new OrderOperation
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Type = type,
            Trailer = trailer,
            Quantity = quantity,
            Unit = unit,
            UnitLabel = unitLabel,
            AppliedAt = appliedAt
        };

        dbContext.OrderOperations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderOperationData(
            entity.Id,
            entity.OrderId,
            entity.Type,
            entity.Trailer,
            entity.Quantity,
            entity.Unit,
            entity.UnitLabel,
            entity.AppliedAt,
            CommentCount: 0,
            PhotoCount: 0);
    }

    public async Task<IReadOnlyList<string>?> SoftDeleteOperationAsync(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        OrderOperation? entity = await dbContext.OrderOperations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Id == operationId && x.OrderId == orderId && !x.IsDeleted,
                cancellationToken);

        if (entity is null)
            return null;

        DateTimeOffset deletedAt = DateTimeOffset.UtcNow;
        entity.IsDeleted = true;
        entity.DeletedAt = deletedAt;

        List<OrderOperationPhoto> photos = await dbContext.OrderOperationPhotos
            .IgnoreQueryFilters()
            .Where(p => p.OperationId == operationId && !p.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (OrderOperationPhoto photo in photos)
        {
            photo.IsDeleted = true;
            photo.DeletedAt = deletedAt;
        }

        List<OrderOperationComment> comments = await dbContext.OrderOperationComments
            .Where(c => c.OperationId == operationId)
            .ToListAsync(cancellationToken);

        dbContext.OrderOperationComments.RemoveRange(comments);

        await dbContext.SaveChangesAsync(cancellationToken);

        return photos.Select(p => p.StorageKey).ToArray();
    }

    public Task<bool> OperationExistsAsync(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        return dbContext.OrderOperations
            .AsNoTracking()
            .AnyAsync(x => x.Id == operationId && x.OrderId == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderOperationCommentData>> GetOperationCommentsAsync(
        Guid operationId,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrderOperationComments
            .AsNoTracking()
            .Where(c => c.OperationId == operationId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new OrderOperationCommentData(
                c.Id,
                c.OperationId,
                c.Text,
                c.AuthorName,
                c.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderOperationCommentData> AddOperationCommentAsync(
        Guid operationId,
        string text,
        string? authorName,
        CancellationToken cancellationToken)
    {
        var entity = new OrderOperationComment
        {
            Id = Guid.NewGuid(),
            OperationId = operationId,
            Text = text,
            AuthorName = authorName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.OrderOperationComments.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderOperationCommentData(
            entity.Id,
            entity.OperationId,
            entity.Text,
            entity.AuthorName,
            entity.CreatedAt);
    }

    public async Task<IReadOnlyList<OrderOperationPhotoData>> GetOperationPhotosAsync(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrderOperationPhotos
            .AsNoTracking()
            .Where(p => p.OperationId == operationId && p.Operation.OrderId == orderId)
            .OrderBy(p => p.Id)
            .Select(p => new OrderOperationPhotoData(
                p.Id,
                orderId,
                p.OperationId,
                p.FileName,
                p.ContentType))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderOperationPhotoData> AddOperationPhotoAsync(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        string fileName,
        string contentType,
        string storageKey,
        CancellationToken cancellationToken)
    {
        var entity = new OrderOperationPhoto
        {
            Id = photoId,
            OperationId = operationId,
            FileName = fileName,
            ContentType = contentType,
            StorageKey = storageKey
        };

        dbContext.OrderOperationPhotos.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderOperationPhotoData(
            entity.Id,
            orderId,
            entity.OperationId,
            entity.FileName,
            entity.ContentType);
    }

    public async Task<OrderOperationPhotoContentData?> GetOperationPhotoContentAsync(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrderOperationPhotos
            .AsNoTracking()
            .Where(p => p.Id == photoId && p.OperationId == operationId && p.Operation.OrderId == orderId)
            .Select(p => new OrderOperationPhotoContentData(
                p.Id,
                p.FileName,
                p.ContentType,
                p.StorageKey))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string?> SoftDeleteOperationPhotoAsync(
        Guid orderId,
        Guid operationId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        OrderOperationPhoto? entity = await dbContext.OrderOperationPhotos
            .IgnoreQueryFilters()
            .Include(p => p.Operation)
            .FirstOrDefaultAsync(
                p => p.Id == photoId
                     && p.OperationId == operationId
                     && p.Operation.OrderId == orderId
                     && !p.IsDeleted,
                cancellationToken);

        if (entity is null)
            return null;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.StorageKey;
    }
}
