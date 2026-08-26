using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhoto;

public sealed record GetOperationPhotoQuery(Guid OrderId, Guid OperationId, Guid PhotoId)
    : IQuery<Result<OrderFileResponse>>;
