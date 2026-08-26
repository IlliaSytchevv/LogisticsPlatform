namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderWarehouseNoteResponse(
    string? Text,
    IReadOnlyList<OrderWarehousePhotoResponse> Photos);
