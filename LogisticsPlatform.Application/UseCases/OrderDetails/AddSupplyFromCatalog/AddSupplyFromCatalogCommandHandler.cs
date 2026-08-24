using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Models.Supplies;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupplyFromCatalog;

public sealed class AddSupplyFromCatalogCommandHandler(
    IOrderDetailsRepository orderDetailsRepository,
    ISupplyCatalogRepository supplyCatalogRepository)
    : ICommandHandler<AddSupplyFromCatalogCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        AddSupplyFromCatalogCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.NotFound();

        SupplyCatalogItemInternalData? catalogItem = await supplyCatalogRepository.GetByIdAsync(
            command.CatalogItemId,
            cancellationToken);

        if (catalogItem is null)
            return Result<OrderSupplyResponse>.NotFound();

        // Client path: always persist platform price only (WP / margin never stored on order line).
        OrderSupplyData data = await orderDetailsRepository.AddSupplyAsync(
            command.OrderId,
            catalogItem.Sku,
            catalogItem.Name,
            catalogItem.Category,
            command.Quantity,
            catalogItem.PlatformPriceCents,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
