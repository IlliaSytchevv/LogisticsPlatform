namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record OrderWarehousePhotoResponse(
    Guid Id,
    string FileName,
    string ContentType,
    int SortOrder,
    string DownloadUrl);
