using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IOrderBolPdfService
{
    Task WriteAsync(OrderDocumentData order, Stream output, CancellationToken cancellationToken);
}

public interface IOrderQrService
{
    Task WritePngAsync(OrderDocumentData order, Stream output, CancellationToken cancellationToken);
}
