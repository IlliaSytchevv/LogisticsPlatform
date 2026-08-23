using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Export;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Orders.ExportOrders;

public sealed record ExportOrdersQuery(
    OrderListTab Tab,
    Guid? HubId,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    OrderStatus? Status,
    string? Search) : IQuery<Result<OrdersExportFileResponse>>;