namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record OrderOperationPhotoResponse(
    Guid Id,
    string FileName,
    string ContentType,
    int SortOrder,
    string DownloadUrl);
