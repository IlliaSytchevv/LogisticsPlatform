using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsPlatform.Application.Abstractions.Messaging;

internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();

    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default)
    {
        Type handlerType = GetHandlerType(command.GetType(), typeof(ICommandHandler<,>), typeof(TResponse));
        object handler = serviceProvider.GetRequiredService(handlerType);
        return InvokeHandler<TResponse>(handler, command, ct);
    }

    public Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
    {
        Type handlerType = GetHandlerType(query.GetType(), typeof(IQueryHandler<,>), typeof(TResponse));
        object handler = serviceProvider.GetRequiredService(handlerType);
        return InvokeHandler<TResponse>(handler, query, ct);
    }

    private static Type GetHandlerType(Type requestType, Type handlerInterfaceType, Type responseType)
    {
        return HandlerTypeCache.GetOrAdd(
            requestType,
            _ => handlerInterfaceType.MakeGenericType(requestType, responseType));
    }

    private static async Task<TResponse> InvokeHandler<TResponse>(object handler, object request, CancellationToken ct)
    {
        MethodInfo handleMethod = handler.GetType().GetMethod("Handle")!;
        return await (Task<TResponse>)handleMethod.Invoke(handler, [request, ct])!;
    }
}
