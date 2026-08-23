using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.List;

namespace LogisticsPlatform.Application.UseCases.Orders.CreateOrder;

public sealed class CreateOrderCommandHandler(IOrdersRepository ordersRepository)
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

        OrderCreatedData created = await ordersRepository.CreateDraftAsync(
            command.Type,
            command.HubId,
            command.CreatedByUserId,
            command.ScheduledAt ?? DateTimeOffset.UtcNow,
            string.IsNullOrWhiteSpace(command.DestinationCity) ? "TBD" : command.DestinationCity.Trim(),
            string.IsNullOrWhiteSpace(command.DestinationRegion) ? "ON" : command.DestinationRegion.Trim(),
            command.PrimaryReference,
            cancellationToken);

        return Result.Success(
            new CreateOrderResponse(created.Id, created.Number, created.Type, created.Status));
    }
}
