using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupplyFromCatalog;

public sealed record AddSupplyFromCatalogCommand(
    Guid OrderId,
    Guid CatalogItemId,
    int Quantity) : ICommand<Result<OrderSupplyResponse>>;
