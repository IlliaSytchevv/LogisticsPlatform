using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;

public sealed class AddOperationPhotoCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<AddOperationPhotoCommand, Result<OrderOperationPhotoResponse>>
{
    public async Task<Result<OrderOperationPhotoResponse>> Handle(
        AddOperationPhotoCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.OperationExistsAsync(
                command.OrderId,
                command.OperationId,
                cancellationToken))
            return Result<OrderOperationPhotoResponse>.NotFound();

        IReadOnlyList<OrderOperationPhotoData> existing =
            await orderDetailsRepository.GetOperationPhotosAsync(
                command.OrderId,
                command.OperationId,
                cancellationToken);

        OrderOperationPhotoData data = await orderDetailsRepository.AddOperationPhotoAsync(
            command.OrderId,
            command.OperationId,
            command.FileName,
            command.ContentType,
            command.Content,
            command.SortOrder ?? existing.Count,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
