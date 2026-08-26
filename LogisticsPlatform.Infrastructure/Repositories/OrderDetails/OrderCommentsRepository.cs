using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Repositories.OrderDetails;

public sealed class OrderCommentsRepository(AppDbContext dbContext) : IOrderCommentsRepository
{
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
