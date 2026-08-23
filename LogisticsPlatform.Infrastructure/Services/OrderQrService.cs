using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using QRCoder;

namespace LogisticsPlatform.Infrastructure.Services;

public sealed class OrderQrService : IOrderQrService
{
    public Task WritePngAsync(OrderDocumentData order, Stream output, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = $"FREITTY:{order.Number}:{order.Id}";
        
        using var generator = new QRCodeGenerator();
        using QRCodeData data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        
        var qr = new PngByteQRCode(data);
        var bytes = qr.GetGraphic(8);
        
        return output.WriteAsync(bytes, cancellationToken).AsTask();
    }
}