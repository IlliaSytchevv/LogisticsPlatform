using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.Interfaces.FileExport;

public interface IOrdersExportSource
{
    IAsyncEnumerable<OrderExportRowData> ReadAsync(
        OrdersListFilter filter,
        CancellationToken cancellationToken);
}
