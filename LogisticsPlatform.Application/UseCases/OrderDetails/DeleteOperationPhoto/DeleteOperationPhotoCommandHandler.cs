using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperationPhoto;

public sealed class DeleteOperationPhotoCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<DeleteOperationPhotoCommand, Result>
{
    public async Task<Result> Handle(DeleteOperationPhotoCommand command, CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.OperationExistsAsync(
                command.OrderId,
                command.OperationId,
                cancellationToken))
            return Result.NotFound();

        bool deleted = await orderDetailsRepository.SoftDeleteOperationPhotoAsync(
            command.OrderId,
            command.OperationId,
            command.PhotoId,
            cancellationToken);

        return deleted ? Result.Success() : Result.NotFound();
    }
}
