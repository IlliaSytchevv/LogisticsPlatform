using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetOperationPhotos;

public sealed record GetOperationPhotosQuery(Guid OrderId, Guid OperationId)
    : IQuery<Result<IReadOnlyList<OrderOperationPhotoResponse>>>;
