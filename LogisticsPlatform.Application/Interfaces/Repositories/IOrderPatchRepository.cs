using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderPatchRepository
{
    Task<OrderStatus?> GetStatusAsync(Guid orderId, CancellationToken cancellationToken);

    Task<bool> PatchOrderAsync(OrderDetailPatchData patch, CancellationToken cancellationToken);
}
