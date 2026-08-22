using System.Runtime.CompilerServices;
using LogisticsPlatform.Application.Interfaces.FileExport;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Infrastructure.Database;
using LogisticsPlatform.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.FileExport;

public sealed class OrdersExportBatchReader(AppDbContext dbContext) : IOrdersExportSource
{
    private const int BatchSize = 500;

    public async IAsyncEnumerable<OrderExportRowData> ReadAsync(
        OrdersListFilter filter,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var skip = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batch = await OrdersQueryFilter
                .Apply(dbContext.Orders.AsNoTracking(), filter)
                .OrderByDescending(o => o.ScheduledAt)
                .ThenByDescending(o => o.Id)
                .Skip(skip)
                .Take(BatchSize)
                .Select(o => new
                {
                    o.Id,
                    o.Number,
                    o.Type,
                    o.Status,
                    Hub = o.Hub.Name,
                    o.ScheduledAt,
                    CarrierName = o.Carrier != null ? o.Carrier.Name : null,
                    CreatedByName = o.CreatedByUser.DisplayName,
                    CreatedByRole = o.CreatedByUser.Role,
                    o.DeclaredQty,
                    o.ActualQty,
                    o.NextActionLabel,
                    o.HasAlert,
                    o.AlertReason
                })
                .ToListAsync(cancellationToken);

            if (batch.Count == 0)
                yield break;

            var orderIds = batch.Select(o => o.Id).ToArray();

            var quantityLines = await dbContext.OrderQuantityLines
                .AsNoTracking()
                .Where(x => orderIds.Contains(x.OrderId))
                .OrderBy(x => x.Unit)
                .Select(x => new { x.OrderId, x.Unit, x.Count })
                .ToListAsync(cancellationToken);

            var references = await dbContext.SubOrders
                .AsNoTracking()
                .Where(x => orderIds.Contains(x.OrderId))
                .OrderBy(x => x.SortOrder)
                .Select(x => new { x.OrderId, x.Reference })
                .ToListAsync(cancellationToken);

            var quantityByOrder = quantityLines
                .GroupBy(x => x.OrderId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => (x.Unit, x.Count)).ToList());

            var referencesByOrder = references
                .GroupBy(x => x.OrderId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Reference).ToList());

            foreach (var row in batch)
            {
                quantityByOrder.TryGetValue(row.Id, out var lines);
                referencesByOrder.TryGetValue(row.Id, out var refs);

                yield return new OrderExportRowData(
                    row.Number,
                    row.Type,
                    row.Status,
                    row.Hub,
                    row.ScheduledAt,
                    row.CarrierName,
                    row.CreatedByName,
                    row.CreatedByRole,
                    row.DeclaredQty,
                    row.ActualQty,
                    FormatQuantity(row.DeclaredQty, row.ActualQty, lines),
                    refs is { Count: > 0 } ? string.Join("; ", refs) : string.Empty,
                    row.NextActionLabel,
                    row.HasAlert,
                    row.AlertReason);
            }

            if (batch.Count < BatchSize)
                yield break;

            skip += BatchSize;
        }
    }

    private static string FormatQuantity(
        int? declared,
        int? actual,
        List<(PalletUnit Unit, int Count)>? lines)
    {
        if (declared.HasValue && actual.HasValue && declared != actual)
            return $"{actual} / {declared}+";

        if (lines is { Count: > 0 })
        {
            return string.Join(
                " + ",
                lines.Select(x =>
                    x.Unit == PalletUnit.XL
                        ? $"{x.Count} XL"
                        : x.Unit == PalletUnit.Standard
                            ? $"{x.Count} Std"
                            : x.Count.ToString()));
        }

        return actual?.ToString() ?? declared?.ToString() ?? string.Empty;
    }
}