using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderPatchRepository
{
    Task<(OrderStatus Status, string Number)?> GetStatusAndNumberAsync(
        Guid orderId,
        CancellationToken cancellationToken);

    Task<bool> NumberExistsAsync(
        string number,
        Guid excludeOrderId,
        CancellationToken cancellationToken);

    Task<bool> PatchOrderAsync(OrderDetailPatchData patch, CancellationToken cancellationToken);
}
