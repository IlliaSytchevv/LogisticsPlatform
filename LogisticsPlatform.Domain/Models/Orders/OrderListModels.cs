using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Models.Orders;

public sealed record OrderListItemData(
    Guid Id,
    string Number,
    OrderType Type,
    OrderStatus Status,
    bool HasAlert,
    string? AlertReason,
    string CreatedByName,
    string CreatedByInitials,
    UserRole CreatedByRole,
    string Hub,
    DateTimeOffset ScheduledAt,
    int? DeclaredQty,
    int? ActualQty,
    string? CarrierName,
    string DestinationCity,
    string DestinationRegion,
    string? DestinationNote,
    string? NextActionLabel,
    NextActionKind? NextActionKind,
    DateTimeOffset? NextActionDueAt,
    long? NextActionAmountCents,
    string? NextActionDocumentNumber,
    IReadOnlyList<OrderListQuantityLineData> QuantityLines,
    IReadOnlyList<OrderListSubOrderData> SubOrders);

public sealed record OrderListQuantityLineData(PalletUnit Unit, int Count);

public sealed record OrderListSubOrderData(
    string Number,
    string Reference,
    int PalletCount,
    bool HasMissingPhoto);

public sealed record OrdersListData(
    int TotalCount,
    IReadOnlyList<OrderListItemData> Items);

public sealed record OrdersTabCountsData(
    int All,
    int CrossDock,
    int Consolidation,
    int Alerts,
    int Drafts);

public sealed record OrdersFilterOptionsData(
    IReadOnlyList<OrderHubOptionData> Hubs);

public sealed record OrderHubOptionData(Guid Id, string Name);

public sealed record OrdersListFilter(
    OrderListTab? Tab,
    Guid? HubId,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    OrderStatus? Status,
    string? Search);

public sealed record OrderCreatedData(
    Guid Id,
    string Number,
    OrderType Type,
    OrderStatus Status);

