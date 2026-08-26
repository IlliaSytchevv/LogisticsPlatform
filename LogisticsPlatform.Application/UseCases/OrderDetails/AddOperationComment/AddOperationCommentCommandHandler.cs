using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationComment;

public sealed class AddOperationCommentCommandHandler(IOrderOperationsRepository orderOperationsRepository)
    : ICommandHandler<AddOperationCommentCommand, Result<OrderCommentResponse>>
{
    public async Task<Result<OrderCommentResponse>> Handle(
        AddOperationCommentCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderOperationsRepository.OperationExistsAsync(
                command.OrderId,
                command.OperationId,
                cancellationToken))
            return Result<OrderCommentResponse>.NotFound();

        OrderOperationCommentData data = await orderOperationsRepository.AddOperationCommentAsync(
            command.OperationId,
            command.Text,
            command.AuthorName,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
