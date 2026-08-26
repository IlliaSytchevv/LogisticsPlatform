using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteSupply;

public sealed class DeleteOrderSupplyCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderSuppliesRepository orderSuppliesRepository)
    : ICommandHandler<DeleteOrderSupplyCommand, Result>
{
    public async Task<Result> Handle(DeleteOrderSupplyCommand command, CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result.NotFound();

        bool deleted = await orderSuppliesRepository.SoftDeleteSupplyAsync(
            command.OrderId,
            command.SupplyId,
            cancellationToken);

        return deleted ? Result.Success() : Result.NotFound();
    }
}
