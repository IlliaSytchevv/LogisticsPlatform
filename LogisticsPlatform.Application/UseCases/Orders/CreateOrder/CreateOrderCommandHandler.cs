using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Application.Models.Supplies;
using LogisticsPlatform.Domain.DTO.Orders.List;

namespace LogisticsPlatform.Application.UseCases.Orders.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrdersRepository ordersRepository,
    ISupplyCatalogRepository supplyCatalogRepository,
    IOrderDetailsRepository orderDetailsRepository)
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

        if (command.Supplies is { Count: > 0 })
        {
            foreach (CreateOrderSupplyLineRequest line in command.Supplies)
            {
                SupplyCatalogItemInternalData? item = await supplyCatalogRepository.GetByIdAsync(
                    line.CatalogItemId,
                    cancellationToken);
                if (item is null)
                    return Result<CreateOrderResponse>.NotFound();
            }
        }

        OrderCreatedData created = await ordersRepository.CreateDraftAsync(
            command.Type,
            command.HubId,
            command.CreatedByUserId,
            command.ScheduledAt ?? DateTimeOffset.UtcNow,
            string.IsNullOrWhiteSpace(command.DestinationCity) ? "TBD" : command.DestinationCity.Trim(),
            string.IsNullOrWhiteSpace(command.DestinationRegion) ? "ON" : command.DestinationRegion.Trim(),
            command.PrimaryReference,
            cancellationToken);

        if (command.Supplies is { Count: > 0 })
        {
            foreach (CreateOrderSupplyLineRequest line in command.Supplies)
            {
                SupplyCatalogItemInternalData item = (await supplyCatalogRepository.GetByIdAsync(
                    line.CatalogItemId,
                    cancellationToken))!;

                await orderDetailsRepository.AddSupplyAsync(
                    created.Id,
                    item.Sku,
                    item.Name,
                    item.Category,
                    line.Quantity,
                    item.PlatformPriceCents,
                    cancellationToken);
            }
        }

        return Result.Success(
            new CreateOrderResponse(created.Id, created.Number, created.Type, created.Status));
    }
}
