namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderWarehousePhotoResponse(
    Guid Id,
    string FileName,
    string ContentType,
    string DownloadUrl);
