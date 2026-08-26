using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Domain.Orders;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderWarehousePhotosRepository(AppDbContext dbContext) : IOrderWarehousePhotosRepository
{
    public async Task<OrderWarehousePhotoData> AddWarehousePhotoAsync(
        Guid orderId,
        Guid photoId,
        string fileName,
        string contentType,
        string storageKey,
        CancellationToken cancellationToken)
    {
        var entity = new OrderWarehousePhoto
        {
            Id = photoId,
            OrderId = orderId,
            FileName = fileName,
            ContentType = contentType,
            StorageKey = storageKey
        };

        dbContext.OrderWarehousePhotos.Add(entity);

        Order? order = await dbContext.Orders
            .Include(o => o.SubOrders)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is not null)
            ClearPhotoMissingState(order);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderWarehousePhotoData(
            entity.Id,
            entity.OrderId,
            entity.FileName,
            entity.ContentType);
    }

    public async Task<OrderWarehousePhotoContentData?> GetWarehousePhotoContentAsync(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrderWarehousePhotos
            .AsNoTracking()
            .Where(p => p.Id == photoId && p.OrderId == orderId)
            .Select(p => new OrderWarehousePhotoContentData(
                p.Id,
                p.FileName,
                p.ContentType,
                p.StorageKey))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string?> SoftDeleteWarehousePhotoAsync(
        Guid orderId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        OrderWarehousePhoto? entity = await dbContext.OrderWarehousePhotos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Id == photoId && x.OrderId == orderId && !x.IsDeleted,
                cancellationToken);

        if (entity is null)
            return null;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        bool anyPhotosLeft = await dbContext.OrderWarehousePhotos
            .IgnoreQueryFilters()
            .AnyAsync(
                p => p.OrderId == orderId && p.Id != photoId && !p.IsDeleted,
                cancellationToken);

        if (!anyPhotosLeft)
        {
            Order? order = await dbContext.Orders
                .Include(o => o.SubOrders)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order is not null && RequiresWarehousePhoto(order))
                RaisePhotoMissingState(order);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.StorageKey;
    }

    private static bool RequiresWarehousePhoto(Order order) =>
        order.NextAction.NextActionKind == NextActionKind.UploadPhoto
        || order.Status == OrderStatus.Alert
        || string.Equals(order.AlertReason, OrderAlertReasons.PhotoMissing, StringComparison.OrdinalIgnoreCase)
        || (!string.IsNullOrWhiteSpace(order.Cabinet.ServicesCsv)
            && order.Cabinet.ServicesCsv.Contains("Photo", StringComparison.OrdinalIgnoreCase));

    private static void ClearPhotoMissingState(Order order)
    {
        foreach (SubOrder subOrder in order.SubOrders)
            subOrder.HasMissingPhoto = false;

        if (string.Equals(order.AlertReason, OrderAlertReasons.PhotoMissing, StringComparison.OrdinalIgnoreCase))
        {
            order.HasAlert = false;
            order.AlertReason = null;
        }

        if (order.Status == OrderStatus.Alert)
            order.Status = OrderStatus.InProgress;

        if (order.NextAction.NextActionKind == NextActionKind.UploadPhoto)
        {
            order.NextAction.AwaitingClientAction = false;
            order.NextAction.NextActionKind = NextActionKind.WaitingForTruck;
            order.NextAction.NextActionLabel = "Waiting for truck";
        }
    }

    private static void RaisePhotoMissingState(Order order)
    {
        order.HasAlert = true;
        order.AlertReason = OrderAlertReasons.PhotoMissing;

        if (order.Status is OrderStatus.New or OrderStatus.InProgress)
            order.Status = OrderStatus.Alert;

        order.NextAction.AwaitingClientAction = true;
        order.NextAction.NextActionKind = NextActionKind.UploadPhoto;
        order.NextAction.NextActionLabel = "Upload photo";

        SubOrder? target = order.SubOrders
            .OrderByDescending(s => s.SortOrder)
            .FirstOrDefault();

        if (target is not null)
            target.HasMissingPhoto = true;
    }
}
