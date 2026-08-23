using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.Models.Orders;

public sealed record OrderDetailPatchData(
    Guid OrderId,
    string? CustomerName,
    bool SetCustomerName,
    string? PrimaryReference,
    bool SetPrimaryReference,
    Guid? HubId,
    DateTimeOffset? ScheduledAt,
    int? DeclaredQty,
    bool SetDeclaredQty,
    int? ActualQty,
    bool SetActualQty,
    string? TrailerType,
    bool SetTrailerType,
    Guid? CarrierId,
    bool SetCarrierId,
    string? Phone,
    bool SetPhone,
    string? TruckNumber,
    bool SetTruckNumber,
    string? TrailerNumber,
    bool SetTrailerNumber,
    string? DockCode,
    bool SetDockCode,
    string? DockBay,
    bool SetDockBay,
    DateTimeOffset? DockAssignedAt,
    bool SetDockAssignedAt,
    Guid? AssignedToUserId,
    bool SetAssignedToUserId,
    string? WarehouseNote,
    bool SetWarehouseNote,
    string? StockStatusLabel,
    bool SetStockStatusLabel,
    string? LoadingStatusLabel,
    bool SetLoadingStatusLabel,
    string? ServicesCsv,
    bool SetServicesCsv,
    string? QuantityUnitLabel,
    bool SetQuantityUnitLabel,
    string? DockStatusLabel,
    bool SetDockStatusLabel,
    OrderStatus? Status,
    bool? HasAlert,
    string? AlertReason,
    bool SetAlertReason);

public sealed record OrderOperationData(
    Guid Id,
    Guid OrderId,
    OrderOperationType Type,
    string? Trailer,
    int Quantity,
    PalletUnit Unit,
    string? UnitLabel,
    DateTimeOffset AppliedAt,
    int CommentCount,
    int PhotoCount);

public sealed record OrderOperationCommentData(
    Guid Id,
    Guid OperationId,
    string Text,
    string? AuthorName,
    DateTimeOffset CreatedAt);

public sealed record OrderOperationPhotoData(
    Guid Id,
    Guid OrderId,
    Guid OperationId,
    string FileName,
    string ContentType,
    int SortOrder);

public sealed record OrderOperationPhotoContentData(
    Guid Id,
    string FileName,
    string ContentType,
    byte[] Content);

public sealed record OrderSupplyData(
    Guid Id,
    Guid OrderId,
    string Sku,
    string Name,
    string Category,
    int Quantity,
    long UnitPriceCents,
    long LineTotalCents);

public sealed record OrderWarehousePhotoData(
    Guid Id,
    Guid OrderId,
    string FileName,
    string ContentType,
    int SortOrder);

public sealed record OrderWarehousePhotoContentData(
    Guid Id,
    string FileName,
    string ContentType,
    byte[] Content);

public sealed record OrderCommentData(
    Guid Id,
    Guid OrderId,
    string Text,
    string? AuthorName,
    DateTimeOffset CreatedAt);

public sealed record OrderTimelineEntryData(
    Guid Id,
    Guid OrderId,
    string Kind,
    string Text,
    string? AuthorName,
    DateTimeOffset CreatedAt);

public sealed record OrderHubDockData(
    string Code,
    string? BayLabel);

public sealed record OrderDetailsData(
    Guid Id,
    string Number,
    OrderType Type,
    OrderStatus Status,
    string? PrimaryReference,
    string? CustomerName,
    string? Phone,
    Guid HubId,
    string HubName,
    string? HubRegionCode,
    DateTimeOffset ScheduledAt,
    Guid? CarrierId,
    string? CarrierName,
    string? TrailerType,
    string? TruckNumber,
    string? TrailerNumber,
    Guid? AssignedToUserId,
    string? AssignedToUserName,
    string? ServicesCsv,
    string? StockStatusLabel,
    string? LoadingStatusLabel,
    bool HasAlert,
    string? AlertReason,
    string? DockCode,
    string? DockBay,
    DateTimeOffset? DockAssignedAt,
    string? DockStatusLabel,
    int? DeclaredQty,
    int? ActualQty,
    string? QuantityUnitLabel,
    string? WarehouseNote,
    IReadOnlyList<OrderHubDockData> HubDocks,
    IReadOnlyList<OrderWarehousePhotoData> WarehousePhotos,
    IReadOnlyList<OrderOperationData> Operations,
    IReadOnlyList<OrderSupplyData> Supplies);

public sealed record OrderDocumentData(
    Guid Id,
    string Number,
    string? PrimaryReference,
    string? CustomerName,
    string? Phone,
    string HubName,
    string? CarrierName,
    DateTimeOffset ScheduledAt,
    int? DeclaredQty,
    int? ActualQty,
    string? TruckNumber,
    string? TrailerNumber,
    string? DockCode,
    string? DockBay);
