using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetBolPdf;

public sealed class GetOrderBolPdfQueryHandler(
    IOrderDetailsRepository orderDetailsRepository,
    IOrderBolPdfService bolPdfService)
    : IQueryHandler<GetOrderBolPdfQuery, Result<OrderFileResponse>>
{
    public async Task<Result<OrderFileResponse>> Handle(
        GetOrderBolPdfQuery query,
        CancellationToken cancellationToken)
    {
        OrderDocumentData? order = await orderDetailsRepository.GetDocumentDataAsync(
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