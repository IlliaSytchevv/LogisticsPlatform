using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOrderDetails;

public sealed record GetOrderDetailsQuery(Guid OrderId) : IQuery<Result<OrderDetailsResponse>>;
