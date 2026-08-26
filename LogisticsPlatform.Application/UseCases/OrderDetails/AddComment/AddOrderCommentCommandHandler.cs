using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;
using LogisticsPlatform.Application.Extensions.Mapping.OrderDetails;
using LogisticsPlatform.Application.Interfaces.Repositories;
using LogisticsPlatform.Application.Models.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddComment;

public sealed class AddOrderCommentCommandHandler(
    IOrderAccessRepository orderAccessRepository,
    IOrderCommentsRepository orderCommentsRepository)
    : ICommandHandler<AddOrderCommentCommand, Result<OrderCommentResponse>>
{
    public async Task<Result<OrderCommentResponse>> Handle(
        AddOrderCommentCommand command,
        CancellationToken cancellationToken)
    {
        if (!await orderAccessRepository.ExistsAsync(command.OrderId, cancellationToken))
            return Result<OrderCommentResponse>.NotFound();

        OrderCommentData data = await orderCommentsRepository.AddCommentAsync(
            command.OrderId,
            command.Text,
            command.AuthorName,
            cancellationToken);

        return Result.Success(OrderDetailsMapper.ToResponse(data));
    }
}
