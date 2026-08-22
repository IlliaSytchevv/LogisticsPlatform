namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record OrderWarehousePhotoResponse(
    Guid Id,
    string Url,
    int SortOrder);
