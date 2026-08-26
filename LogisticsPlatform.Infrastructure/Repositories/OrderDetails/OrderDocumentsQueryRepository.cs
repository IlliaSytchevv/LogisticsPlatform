using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderDocumentsQueryRepository(AppDbContext dbContext) : IOrderDocumentsQueryRepository
{
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
}
