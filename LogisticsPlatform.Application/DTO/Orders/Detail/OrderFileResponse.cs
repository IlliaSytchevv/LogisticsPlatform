namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderFileResponse(
    string FileName,
    string ContentType,
    Func<Stream, CancellationToken, Task> WriteToAsync);
