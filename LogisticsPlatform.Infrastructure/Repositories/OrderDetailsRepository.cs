using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories;

public sealed class OrderDetailsRepository(AppDbContext dbContext) : IOrderDetailsRepository
{
    public Task<bool> ExistsAsync(Guid orderId, CancellationToken cancellationToken) =>
        dbContext.Orders.AsNoTracking().AnyAsync(o => o.Id == orderId, cancellationToken);

    public async Task<OrderDetailsData?> GetDetailsAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new
            {
                o.Id,
                o.Number,
                o.Type,
                o.Status,
                o.PrimaryReference,
                o.CustomerName,
                o.Phone,
                o.HubId,
                HubName = o.Hub.Name,
                HubRegionCode = o.Hub.RegionCode,
                o.ScheduledAt,
                o.CarrierId,
                CarrierName = o.Carrier != null ? o.Carrier.Name : null,
                o.TrailerType,
                o.TruckNumber,
                o.TrailerNumber,
                o.AssignedToUserId,
                AssignedToUserName = o.AssignedToUser != null ? o.AssignedToUser.DisplayName : null,
                o.ServicesCsv,
                o.StockStatusLabel,
                o.LoadingStatusLabel,
                o.HasAlert,
                o.AlertReason,
                o.DockCode,
                o.DockBay,
                o.DockAssignedAt,
                o.DockStatusLabel,
                o.DeclaredQty,
                o.ActualQty,
                o.QuantityUnitLabel,
                o.WarehouseNote
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
            return null;

        List<OrderHubDockData> hubDocks = await dbContext.HubDocks
            .AsNoTracking()
            .Where(d => d.HubId == order.HubId)
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Code)
            .Select(d => new OrderHubDockData(d.Code, d.BayLabel))
            .ToListAsync(cancellationToken);

        List<OrderWarehousePhotoData> photos = await dbContext.OrderWarehousePhotos
            .AsNoTracking()
            .Where(p => p.OrderId == orderId)
            .OrderBy(p => p.SortOrder)
            .Select(p => new OrderWarehousePhotoData(
                p.Id,
                p.OrderId,
                p.FileName,
                p.ContentType,
                p.SortOrder))
            .ToListAsync(cancellationToken);

        List<OrderOperationData> operations = await dbContext.OrderOperations
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.AppliedAt)
            .Select(x => new OrderOperationData(
                x.Id,
                x.OrderId,
                x.Type,
                x.Trailer,
                x.Quantity,
                x.Unit,
                x.UnitLabel,
                x.AppliedAt))
            .ToListAsync(cancellationToken);

        List<OrderSupplyData> supplies = await dbContext.OrderSupplies
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.Name)
            .Select(x => new OrderSupplyData(
                x.Id,
                x.OrderId,
                x.Sku,
                x.Name,
                x.Category,
                x.Quantity,
                x.UnitPriceCents,
                x.LineTotalCents))
            .ToListAsync(cancellationToken);

        return new OrderDetailsData(
            order.Id,
            order.Number,
            order.Type,
            order.Status,
            order.PrimaryReference,
            order.CustomerName,
            order.Phone,
            order.HubId,
            order.HubName,
            order.HubRegionCode,
            order.ScheduledAt,
            order.CarrierId,
            order.CarrierName,
            order.TrailerType,
            order.TruckNumber,
            order.TrailerNumber,
            order.AssignedToUserId,
            order.AssignedToUserName,
            order.ServicesCsv,
            order.StockStatusLabel,
            order.LoadingStatusLabel,
            order.HasAlert,
            order.AlertReason,
            order.DockCode,
            order.DockBay,
            order.DockAssignedAt,
            order.DockStatusLabel,
            order.DeclaredQty,
            order.ActualQty,
            order.QuantityUnitLabel,
            order.WarehouseNote,
            hubDocks,
            photos,
            operations,
            supplies);
    }

    public async Task<OrderDocumentData?> GetDocumentDataAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new OrderDocumentData(
                o.Id,
                o.Number,
                o.PrimaryReference,
                o.CustomerName,
                o.Phone,
                o.Hub.Name,
                o.Carrier != null ? o.Carrier.Name : null,
                o.ScheduledAt,
                o.DeclaredQty,
                o.ActualQty,
                o.TruckNumber,
                o.TrailerNumber,
                o.DockCode,
                o.DockBay))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> PatchOrderAsync(OrderDetailPatchData patch, CancellationToken cancellationToken)
    {
        Order? order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == patch.OrderId, cancellationToken);
        if (order is null)
            return false;

        if (patch.SetCustomerName) order.CustomerName = patch.CustomerName;
        if (patch.SetPrimaryReference) order.PrimaryReference = patch.PrimaryReference;
        if (patch.HubId.HasValue) order.HubId = patch.HubId.Value;
        if (patch.ScheduledAt.HasValue) order.ScheduledAt = patch.ScheduledAt.Value;
        if (patch.SetDeclaredQty) order.DeclaredQty = patch.DeclaredQty;
        if (patch.SetActualQty) order.ActualQty = patch.ActualQty;
        if (patch.SetTrailerType) order.TrailerType = patch.TrailerType;
        if (patch.SetCarrierId) order.CarrierId = patch.CarrierId;
        if (patch.SetPhone) order.Phone = patch.Phone;
        if (patch.SetTruckNumber) order.TruckNumber = patch.TruckNumber;
        if (patch.SetTrailerNumber) order.TrailerNumber = patch.TrailerNumber;
        if (patch.SetDockCode) order.DockCode = patch.DockCode;
        if (patch.SetDockBay) order.DockBay = patch.DockBay;
        if (patch.SetDockAssignedAt) order.DockAssignedAt = patch.DockAssignedAt;
        if (patch.SetAssignedToUserId) order.AssignedToUserId = patch.AssignedToUserId;
        if (patch.SetWarehouseNote) order.WarehouseNote = patch.WarehouseNote;
        if (patch.SetStockStatusLabel) order.StockStatusLabel = patch.StockStatusLabel;
        if (patch.SetLoadingStatusLabel) order.LoadingStatusLabel = patch.LoadingStatusLabel;
        if (patch.SetServicesCsv) order.ServicesCsv = patch.ServicesCsv;
        if (patch.SetQuantityUnitLabel) order.QuantityUnitLabel = patch.QuantityUnitLabel;
        if (patch.SetDockStatusLabel) order.DockStatusLabel = patch.DockStatusLabel;
        if (patch.Status.HasValue) order.Status = patch.Status.Value;
        if (patch.HasAlert.HasValue) order.HasAlert = patch.HasAlert.Value;
        if (patch.SetAlertReason) order.AlertReason = patch.AlertReason;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

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
            entity.AppliedAt);
    }

    public async Task<bool> SoftDeleteOperationAsync(
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
            return false;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<OrderSupplyData> AddSupplyAsync(
        Guid orderId,
        string sku,
        string name,
        string category,
        int quantity,
        long unitPriceCents,
        CancellationToken cancellationToken)
    {
        var entity = new OrderSupply
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Sku = sku,
            Name = name,
            Category = category,
            Quantity = quantity,
            UnitPriceCents = unitPriceCents,
            LineTotalCents = quantity * unitPriceCents
        };

        dbContext.OrderSupplies.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderSupplyData(
            entity.Id,
            entity.OrderId,
            entity.Sku,
            entity.Name,
            entity.Category,
            entity.Quantity,
            entity.UnitPriceCents,
            entity.LineTotalCents);
    }

    public async Task<bool> SoftDeleteSupplyAsync(
        Guid orderId,
        Guid supplyId,
        CancellationToken cancellationToken)
    {
        OrderSupply? entity = await dbContext.OrderSupplies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Id == supplyId && x.OrderId == orderId && !x.IsDeleted,
                cancellationToken);

        if (entity is null)
            return false;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<int> CountWarehousePhotosAsync(Guid orderId, CancellationToken cancellationToken) =>
        dbContext.OrderWarehousePhotos
            .AsNoTracking()
            .CountAsync(p => p.OrderId == orderId, cancellationToken);

    public async Task<OrderWarehousePhotoData> AddWarehousePhotoAsync(
        Guid orderId,
        string fileName,
        string contentType,
        byte[] content,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var entity = new OrderWarehousePhoto
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            FileName = fileName,
            ContentType = contentType,
            Content = content,
            SortOrder = sortOrder
        };

        dbContext.OrderWarehousePhotos.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderWarehousePhotoData(
            entity.Id,
            entity.OrderId,
            entity.FileName,
            entity.ContentType,
            entity.SortOrder);
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
                p.Content))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteWarehousePhotoAsync(
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
            return false;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<OrderCommentData>> GetCommentsAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrderComments
            .AsNoTracking()
            .Where(c => c.OrderId == orderId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new OrderCommentData(c.Id, c.OrderId, c.Text, c.AuthorName, c.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderCommentData> AddCommentAsync(
        Guid orderId,
        string text,
        string? authorName,
        CancellationToken cancellationToken)
    {
        var entity = new OrderComment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Text = text,
            AuthorName = authorName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.OrderComments.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderCommentData(
            entity.Id,
            entity.OrderId,
            entity.Text,
            entity.AuthorName,
            entity.CreatedAt);
    }
}
