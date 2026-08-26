using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderSuppliesRepository(AppDbContext dbContext) : IOrderSuppliesRepository
{
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
}
