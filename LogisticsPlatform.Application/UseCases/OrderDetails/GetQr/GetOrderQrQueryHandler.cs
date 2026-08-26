using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetQr;

public sealed class GetOrderQrQueryHandler(
    IOrderDocumentsQueryRepository orderDocumentsQueryRepository,
    IOrderQrService qrService)
    : IQueryHandler<GetOrderQrQuery, Result<OrderFileResponse>>
{
    public async Task<Result<OrderFileResponse>> Handle(
        GetOrderQrQuery query,
        CancellationToken cancellationToken)
    {
        OrderDocumentData? order = await orderDocumentsQueryRepository.GetDocumentDataAsync(
            query.OrderId,
            cancellationToken);

        if (order is null)
            return Result<OrderFileResponse>.NotFound();

        var file = new OrderFileResponse(
            $"{order.Number}-qr.png",
            "image/png",
            (stream, ct) => qrService.WritePngAsync(order, stream, ct));

        return Result.Success(file);
    }
}
