using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetBolPdf;

public sealed record GetOrderBolPdfQuery(Guid OrderId) : IQuery<Result<OrderFileResponse>>;
