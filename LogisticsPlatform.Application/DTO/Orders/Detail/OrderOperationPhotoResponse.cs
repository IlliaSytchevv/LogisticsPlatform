namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderOperationPhotoResponse(
    Guid Id,
    string FileName,
    string ContentType,
    string DownloadUrl);
