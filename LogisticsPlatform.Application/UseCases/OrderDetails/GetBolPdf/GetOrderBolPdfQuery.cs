using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetBolPdf;

public sealed record GetOrderBolPdfQuery(Guid OrderId) : IQuery<Result<OrderFileResponse>>;
