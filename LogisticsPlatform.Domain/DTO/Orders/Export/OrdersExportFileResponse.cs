namespace LogisticsPlatform.Domain.DTO.Orders.Export;

public sealed record OrdersExportFileResponse(
    string FileName,
    string ContentType,
    Func<Stream, CancellationToken, Task> WriteToAsync);
