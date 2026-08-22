namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record OrderWarehouseNoteResponse(
    string? Text,
    IReadOnlyList<OrderWarehousePhotoResponse> Photos);
