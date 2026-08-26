using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhotos;

public sealed record GetOperationPhotosQuery(Guid OrderId, Guid OperationId)
    : IQuery<Result<IReadOnlyList<OrderOperationPhotoResponse>>>;
