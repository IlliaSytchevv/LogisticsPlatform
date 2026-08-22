using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddComment;

public sealed class AddOrderCommentCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<AddOrderCommentCommand, Result<OrderCommentResponse>>
{
    public async Task<Result<OrderCommentResponse>> Handle(
        AddOrderCommentCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderCommentResponse>.NotFound();

        OrderCommentData data = await orderDetailsRepository.AddCommentAsync(
            command.OrderId,
            command.Text,
            command.AuthorName,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
