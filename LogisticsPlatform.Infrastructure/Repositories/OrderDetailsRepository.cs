using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
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
                o.Cabinet.PrimaryReference,
                o.Cabinet.CustomerName,
                o.Cabinet.Phone,
                o.HubId,
                HubName = o.Hub.Name,
                HubRegionCode = o.Hub.RegionCode,
                o.ScheduledAt,
                o.CarrierId,
                CarrierName = o.Carrier != null ? o.Carrier.Name : null,
                o.Cabinet.TrailerType,
                o.Cabinet.TruckNumber,
                o.Cabinet.TrailerNumber,
                o.Dock.AssignedToUserId,
                AssignedToUserName = o.Dock.AssignedToUser != null ? o.Dock.AssignedToUser.DisplayName : null,
                o.Cabinet.ServicesCsv,
                o.Cabinet.StockStatusLabel,
                o.Cabinet.LoadingStatusLabel,
                o.HasAlert,
                o.AlertReason,
                o.Dock.DockCode,
                o.Dock.DockBay,
                o.Dock.DockAssignedAt,
                o.Dock.DockStatusLabel,
                o.DeclaredQty,
                o.ActualQty,
                o.Cabinet.QuantityUnitLabel,
                o.Dock.WarehouseNote
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
                x.AppliedAt,
                x.Comments.Count(),
                x.Photos.Count()))
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
                o.Cabinet.PrimaryReference,
                o.Cabinet.CustomerName,
                o.Cabinet.Phone,
                o.Hub.Name,
                o.Carrier != null ? o.Carrier.Name : null,
                o.ScheduledAt,
                o.DeclaredQty,
                o.ActualQty,
                o.Cabinet.TruckNumber,
                o.Cabinet.TrailerNumber,
                o.Dock.DockCode,
                o.Dock.DockBay))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> PatchOrderAsync(OrderDetailPatchData patch, CancellationToken cancellationToken)
    {
        Order? order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == patch.OrderId, cancellationToken);
        if (order is null)
            return false;

        if (patch.CustomerName is not null) order.Cabinet.CustomerName = patch.CustomerName;
        if (patch.PrimaryReference is not null) order.Cabinet.PrimaryReference = patch.PrimaryReference;
        if (patch.DeclaredQty is not null) order.DeclaredQty = patch.DeclaredQty;
        if (patch.ActualQty is not null) order.ActualQty = patch.ActualQty;
        if (patch.TrailerType is not null) order.Cabinet.TrailerType = patch.TrailerType;
        if (patch.Phone is not null) order.Cabinet.Phone = patch.Phone;
        if (patch.TruckNumber is not null) order.Cabinet.TruckNumber = patch.TruckNumber;
        if (patch.TrailerNumber is not null) order.Cabinet.TrailerNumber = patch.TrailerNumber;
        if (patch.DockCode is not null) order.Dock.DockCode = patch.DockCode;
        if (patch.DockBay is not null) order.Dock.DockBay = patch.DockBay;
        if (patch.WarehouseNote is not null) order.Dock.WarehouseNote = patch.WarehouseNote;
        if (patch.StockStatusLabel is not null) order.Cabinet.StockStatusLabel = patch.StockStatusLabel;
        if (patch.LoadingStatusLabel is not null) order.Cabinet.LoadingStatusLabel = patch.LoadingStatusLabel;

        if (patch.Status is not null && patch.Status.Value != order.Status)
        {
            OrderStatus previous = order.Status;
            order.Status = patch.Status.Value;
            dbContext.OrderTimelineEntries.Add(new OrderTimelineEntry
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Kind = "Status",
                Text =
                    $"{OrderDetailsMapper.FormatStatus(previous)} → {OrderDetailsMapper.FormatStatus(order.Status)}",
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

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
            entity.AppliedAt,
            CommentCount: 0,
            PhotoCount: 0);
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

    public async Task<OrderSupplyData?> UpdateSupplyAsync(
        Guid orderId,
        Guid supplyId,
        string sku,
        string name,
        string category,
        int quantity,
        long unitPriceCents,
        CancellationToken cancellationToken)
    {
        OrderSupply? entity = await dbContext.OrderSupplies
            .FirstOrDefaultAsync(x => x.Id == supplyId && x.OrderId == orderId, cancellationToken);

        if (entity is null)
            return null;

        entity.Sku = sku;
        entity.Name = name;
        entity.Category = category;
        entity.Quantity = quantity;
        entity.UnitPriceCents = unitPriceCents;
        entity.LineTotalCents = quantity * unitPriceCents;

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

    public async Task<OrderSupplyData?> UpdateSupplyQuantityAsync(
        Guid orderId,
        Guid supplyId,
        int quantity,
        CancellationToken cancellationToken)
    {
        OrderSupply? entity = await dbContext.OrderSupplies
            .FirstOrDefaultAsync(x => x.Id == supplyId && x.OrderId == orderId, cancellationToken);

        if (entity is null)
            return null;

        entity.Quantity = quantity;
        entity.LineTotalCents = quantity * entity.UnitPriceCents;

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

    public async Task<IReadOnlyList<OrderTimelineEntryData>> GetTimelineAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrderTimelineEntries
            .AsNoTracking()
            .Where(e => e.OrderId == orderId)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new OrderTimelineEntryData(
                e.Id,
                e.OrderId,
                e.Kind,
                e.Text,
                e.AuthorName,
                e.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderTimelineEntryData> AddTimelineEntryAsync(
        Guid orderId,
        string kind,
        string text,
        string? authorName,
        CancellationToken cancellationToken)
    {
        var entity = new OrderTimelineEntry
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Kind = kind,
            Text = text,
            AuthorName = authorName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.OrderTimelineEntries.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderTimelineEntryData(
            entity.Id,
            entity.OrderId,
            entity.Kind,
            entity.Text,
            entity.AuthorName,
            entity.CreatedAt);
    }

    public Task<bool> OperationExistsAsync(
        Guid orderId,
        Guid operationId,
        CancellationToken cancellationToken) =>
        dbContext.OrderOperations
            .AsNoTracking()
            .AnyAsync(x => x.Id == operationId && x.OrderId == orderId, cancellationToken);

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
            .OrderBy(p => p.SortOrder)
            .Select(p => new OrderOperationPhotoData(
                p.Id,
                orderId,
                p.OperationId,
                p.FileName,
                p.ContentType,
                p.SortOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderOperationPhotoData> AddOperationPhotoAsync(
        Guid orderId,
        Guid operationId,
        string fileName,
        string contentType,
        byte[] content,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var entity = new OrderOperationPhoto
        {
            Id = Guid.NewGuid(),
            OperationId = operationId,
            FileName = fileName,
            ContentType = contentType,
            Content = content,
            SortOrder = sortOrder
        };

        dbContext.OrderOperationPhotos.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderOperationPhotoData(
            entity.Id,
            orderId,
            entity.OperationId,
            entity.FileName,
            entity.ContentType,
            entity.SortOrder);
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
                p.Content))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> SoftDeleteOperationPhotoAsync(
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
            return false;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
