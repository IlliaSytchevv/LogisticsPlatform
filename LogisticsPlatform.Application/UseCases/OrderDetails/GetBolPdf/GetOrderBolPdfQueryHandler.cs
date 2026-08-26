using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetBolPdf;

public sealed class GetOrderBolPdfQueryHandler(
    IOrderDocumentsQueryRepository orderDocumentsQueryRepository,
    IOrderBolPdfService bolPdfService)
    : IQueryHandler<GetOrderBolPdfQuery, Result<OrderFileResponse>>
{
    public async Task<Result<OrderFileResponse>> Handle(
        GetOrderBolPdfQuery query,
        CancellationToken cancellationToken)
    {
        OrderDocumentData? order = await orderDocumentsQueryRepository.GetDocumentDataAsync(
            query.OrderId,
            cancellationToken);

        if (order is null)
            return Result<OrderFileResponse>.NotFound();

        var file = new OrderFileResponse(
            $"{order.Number}-bol.pdf",
            "application/pdf",
            (stream, ct) => bolPdfService.WriteAsync(order, stream, ct));

        return Result.Success(file);
    }
}