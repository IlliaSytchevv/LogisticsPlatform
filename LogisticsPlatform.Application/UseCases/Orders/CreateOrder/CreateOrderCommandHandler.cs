using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.List;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Models.Supplies;

namespace LogisticsPlatform.Application.UseCases.Orders.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrdersRepository ordersRepository,
    ISupplyCatalogRepository supplyCatalogRepository)
    : ICommandHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (!await ordersRepository.HubExistsAsync(command.HubId, cancellationToken))
            return Result<CreateOrderResponse>.NotFound();

        if (!await ordersRepository.UserExistsAsync(command.CreatedByUserId, cancellationToken))
            return Result<CreateOrderResponse>.Unauthorized();

        IReadOnlyList<OrderSupplyDraftLine>? supplyLines = null;

        if (command.Supplies is { Count: > 0 })
        {
            Guid[] catalogIds = command.Supplies
                .Select(s => s.CatalogItemId)
                .Distinct()
                .ToArray();

            IReadOnlyList<SupplyCatalogItemInternalData> catalogItems =
                await supplyCatalogRepository.GetByIdsAsync(catalogIds, cancellationToken);

            if (catalogItems.Count != catalogIds.Length)
                return Result<CreateOrderResponse>.NotFound();

            Dictionary<Guid, SupplyCatalogItemInternalData> byId =
                catalogItems.ToDictionary(x => x.Id);

            supplyLines = command.Supplies
                .Select(line =>
                {
                    SupplyCatalogItemInternalData item = byId[line.CatalogItemId];
                    return new OrderSupplyDraftLine(
                        item.Sku,
                        item.Name,
                        item.Category,
                        line.Quantity,
                        item.PlatformPriceCents);
                })
                .ToList();
        }

        OrderCreatedData created = await ordersRepository.CreateDraftAsync(
            command.Type,
            command.HubId,
            command.CreatedByUserId,
            command.ScheduledAt ?? DateTimeOffset.UtcNow,
            string.IsNullOrWhiteSpace(command.DestinationCity) ? "TBD" : command.DestinationCity.Trim(),
            string.IsNullOrWhiteSpace(command.DestinationRegion) ? "ON" : command.DestinationRegion.Trim(),
            command.PrimaryReference,
            supplyLines,
            cancellationToken);

        return Result.Success(
            new CreateOrderResponse(created.Id, created.Number, created.Type, created.Status));
    }
}
