using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetQr;

public sealed record GetOrderQrQuery(Guid OrderId) : IQuery<Result<OrderFileResponse>>;
