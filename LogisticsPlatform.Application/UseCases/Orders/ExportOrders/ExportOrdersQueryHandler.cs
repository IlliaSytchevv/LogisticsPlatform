using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Export;
using LogisticsPlatform.Application.Extensions.Mapping.Orders;
using LogisticsPlatform.Application.Interfaces.FileExport;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.Orders.ExportOrders;

public sealed class ExportOrdersQueryHandler(
    IOrdersExportSource ordersExportSource,
    IFileWriter fileWriter)
    : IQueryHandler<ExportOrdersQuery, Result<OrdersExportFileResponse>>
{
    private static readonly string[] Headers =
    [
        "Number",
        "Type",
        "Status",
        "Hub",
        "ScheduledAt",
        "Carrier",
        "CreatedBy",
        "Role",
        "DeclaredQty",
        "ActualQty",
        "Quantity",
        "References",
        "NextAction",
        "HasAlert",
        "AlertReason"
    ];

    public Task<Result<OrdersExportFileResponse>> Handle(
        ExportOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var filter = new OrdersListFilter(
            query.Tab,
            query.HubId,
            query.DateFrom,
            query.DateTo,
            query.Status,
            query.Search);

        string fileName = $"orders-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";

        var response = new OrdersExportFileResponse(
            fileName,
            "text/csv",
            (stream, ct) => fileWriter.WriteAsync(
                stream,
                "Orders",
                Headers,
                ordersExportSource.ReadAsync(filter, ct),
                OrdersExportRowMapper.MapRow,
                ct));

        return Task.FromResult(Result.Success(response));
    }
}
