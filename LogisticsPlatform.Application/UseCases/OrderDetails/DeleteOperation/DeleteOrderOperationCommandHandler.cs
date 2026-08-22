using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperation;

public sealed class DeleteOrderOperationCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<DeleteOrderOperationCommand, Result>
{
    public async Task<Result> Handle(DeleteOrderOperationCommand command, CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result.NotFound();

        bool deleted = await orderDetailsRepository.SoftDeleteOperationAsync(
            command.OrderId,
            command.OperationId,
            cancellationToken);

        return deleted ? Result.Success() : Result.NotFound();
    }
}
