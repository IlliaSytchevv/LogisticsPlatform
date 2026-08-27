using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Models.Supplies;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupplyFromCatalog;

public sealed class AddSupplyFromCatalogCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderPaymentsRepository orderPaymentsRepository,
    IOrderSuppliesRepository orderSuppliesRepository,
    ISupplyCatalogRepository supplyCatalogRepository)
    : ICommandHandler<AddSupplyFromCatalogCommand, Result<OrderSupplyResponse>>
{
    public async Task<Result<OrderSupplyResponse>> Handle(
        AddSupplyFromCatalogCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.NotFound();

        if (await orderPaymentsRepository.HasPaidAsync(command.OrderId, cancellationToken))
            return Result<OrderSupplyResponse>.Conflict("Cannot modify supplies on a paid order.");

        SupplyCatalogItemInternalData? catalogItem = await supplyCatalogRepository.GetByIdAsync(
            command.CatalogItemId,
            cancellationToken);

        if (catalogItem is null)
            return Result<OrderSupplyResponse>.NotFound();

        OrderSupplyData data = await orderSuppliesRepository.AddSupplyAsync(
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
