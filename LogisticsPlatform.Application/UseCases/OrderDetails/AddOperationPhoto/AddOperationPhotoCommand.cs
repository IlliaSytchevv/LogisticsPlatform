using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;

public sealed record AddOperationPhotoCommand(
    Guid OrderId,
    Guid OperationId,
    string FileName,
    string ContentType,
    byte[] Content,
    int? SortOrder) : ICommand<Result<OrderOperationPhotoResponse>>;
