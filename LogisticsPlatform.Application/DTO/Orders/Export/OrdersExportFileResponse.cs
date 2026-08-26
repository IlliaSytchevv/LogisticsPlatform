namespace LogisticsPlatform.Application.DTO.Orders.Export;

public sealed record OrdersExportFileResponse(
    string FileName,
    string ContentType,
    Func<Stream, CancellationToken, Task> WriteToAsync);
