using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationComment;

public sealed class AddOperationCommentCommandHandler(IOrderDetailsRepository orderDetailsRepository)
    : ICommandHandler<AddOperationCommentCommand, Result<OrderCommentResponse>>
{
    public async Task<Result<OrderCommentResponse>> Handle(
        AddOperationCommentCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderDetailsRepository.OperationExistsAsync(
                command.OrderId,
                command.OperationId,
                cancellationToken))
            return Result<OrderCommentResponse>.NotFound();

        OrderOperationCommentData data = await orderDetailsRepository.AddOperationCommentAsync(
            command.OperationId,
            command.Text,
            command.AuthorName,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
