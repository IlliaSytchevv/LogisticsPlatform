using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;

public sealed record AddOperationPhotoCommand(
    Guid OrderId,
    Guid OperationId,
    string FileName,
    string ContentType,
    byte[] Content) : ICommand<Result<OrderOperationPhotoResponse>>;
