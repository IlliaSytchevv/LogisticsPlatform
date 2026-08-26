using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderDocumentsQueryRepository
{
    Task<OrderDocumentData?> GetDocumentDataAsync(Guid orderId, CancellationToken cancellationToken);
}
