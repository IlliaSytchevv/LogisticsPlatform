namespace LogisticsPlatform.Application.Abstractions.Messaging;

public interface IDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);

    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
}
