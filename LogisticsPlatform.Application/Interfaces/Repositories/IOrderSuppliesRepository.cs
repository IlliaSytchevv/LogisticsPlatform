using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderSuppliesRepository
{
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

    Task<bool> SoftDeleteSupplyAsync(
        Guid orderId,
        Guid supplyId,
        CancellationToken cancellationToken);
}
