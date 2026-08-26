namespace LogisticsPlatform.Application.Interfaces.Repositories;

public interface IOrderAccessRepository
{
    Task<bool> ExistsAsync(Guid orderId, CancellationToken cancellationToken);
}
