using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Interfaces.Repositories;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteWarehousePhoto;

public sealed class DeleteWarehousePhotoCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<DeleteWarehousePhotoCommand, Result>
{
    public async Task<Result> Handle(DeleteWarehousePhotoCommand command, CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result.NotFound();

        bool deleted = await orderDetailsRepository.SoftDeleteWarehousePhotoAsync(
            command.OrderId,
            command.PhotoId,
            cancellationToken);

        return deleted ? Result.Success() : Result.NotFound();
    }
}
