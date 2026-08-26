using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderDetailsQueryRepository
{
    Task<OrderDetailsData?> GetDetailsAsync(Guid orderId, CancellationToken cancellationToken);
}
