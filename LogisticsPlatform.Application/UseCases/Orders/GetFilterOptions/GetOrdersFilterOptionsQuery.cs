using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.FilterOptions;

namespace LogisticsPlatform.Application.UseCases.Orders.GetFilterOptions;

public sealed record GetOrdersFilterOptionsQuery : IQuery<Result<OrdersFilterOptionsResponse>>;
