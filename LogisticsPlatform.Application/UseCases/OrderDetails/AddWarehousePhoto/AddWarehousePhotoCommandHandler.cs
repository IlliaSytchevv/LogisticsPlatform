using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

public sealed class AddWarehousePhotoCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<AddWarehousePhotoCommand, Result<OrderWarehousePhotoResponse>>
{
    public async Task<Result<OrderWarehousePhotoResponse>> Handle(
        AddWarehousePhotoCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderWarehousePhotoResponse>.NotFound();

        int count = await orderDetailsRepository.CountWarehousePhotosAsync(
            command.OrderId,
            cancellationToken);

        if (count >= AddWarehousePhotoCommandValidator.MaxPhotosPerOrder)
        {
            return Result<OrderWarehousePhotoResponse>.Invalid(
            [
                new ValidationError(
                    "Content",
                    $"Order already has {AddWarehousePhotoCommandValidator.MaxPhotosPerOrder} photos.")
            ]);
        }

        OrderWarehousePhotoData data = await orderDetailsRepository.AddWarehousePhotoAsync(
            command.OrderId,
            command.FileName,
            command.ContentType,
            command.Content,
            command.SortOrder ?? count,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
