using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderCommentsRepository
{
    Task<IReadOnlyList<OrderCommentData>> GetCommentsAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<OrderCommentData> AddCommentAsync(
        Guid orderId,
        string text,
        string? authorName,
        CancellationToken cancellationToken);
}
