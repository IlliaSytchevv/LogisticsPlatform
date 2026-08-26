using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderPatchRepository(
    AppDbContext dbContext,
    INotificationsFeedCacheInvalidator notificationsFeedCacheInvalidator) : IOrderPatchRepository
{
    public async Task<OrderStatus?> GetStatusAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => (OrderStatus?)o.Status)
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
                Text = string.Empty,
                PreviousStatus = previous,
                NewStatus = patch.Status.Value,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await notificationsFeedCacheInvalidator.InvalidateAsync(cancellationToken);

        return true;
    }
}
