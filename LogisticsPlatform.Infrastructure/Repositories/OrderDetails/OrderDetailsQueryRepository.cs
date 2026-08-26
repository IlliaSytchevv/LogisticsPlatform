using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderDetailsQueryRepository(AppDbContext dbContext) : IOrderDetailsQueryRepository
{
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
            .OrderBy(p => p.Id)
            .Select(p => new OrderWarehousePhotoData(
                p.Id,
                p.OrderId,
                p.FileName,
                p.ContentType))
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

        bool isPaid = await dbContext.OrderPayments
            .AsNoTracking()
            .AnyAsync(
                p => p.OrderId == orderId && p.Status == OrderPaymentStatus.Paid,
                cancellationToken);

        return new OrderDetailsData(
            order.Id, order.Number,
            order.Type, order.Status,
            order.PrimaryReference, order.CustomerName,
            order.Phone, order.HubId,
            order.HubName, order.HubRegionCode,
            order.ScheduledAt, order.CarrierId,
            order.CarrierName, order.TrailerType,
            order.TruckNumber, order.TrailerNumber,
            order.AssignedToUserId, order.AssignedToUserName,
            order.ServicesCsv, order.StockStatusLabel,
            order.LoadingStatusLabel, order.HasAlert,
            order.AlertReason, order.DockCode,
            order.DockBay, order.DockAssignedAt,
            order.DockStatusLabel, order.DeclaredQty,
            order.ActualQty, order.QuantityUnitLabel,
            order.WarehouseNote, hubDocks,
            photos, operations, supplies, isPaid);
    }
}
