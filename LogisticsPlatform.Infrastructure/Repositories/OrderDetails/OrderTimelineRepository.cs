using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderTimelineRepository(AppDbContext dbContext) : IOrderTimelineRepository
{
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
                e.CreatedAt,
                e.PreviousStatus,
                e.NewStatus))
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

        return ToData(entity);
    }

    public async Task<OrderTimelineEntryData> AddStatusChangeAsync(
        Guid orderId,
        OrderStatus? previousStatus,
        OrderStatus newStatus,
        CancellationToken cancellationToken)
    {
        var entity = new OrderTimelineEntry
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Kind = "Status",
            Text = string.Empty,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.OrderTimelineEntries.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToData(entity);
    }

    private static OrderTimelineEntryData ToData(OrderTimelineEntry entity) =>
        new(
            entity.Id,
            entity.OrderId,
            entity.Kind,
            entity.Text,
            entity.AuthorName,
            entity.CreatedAt,
            entity.PreviousStatus,
            entity.NewStatus);
}
