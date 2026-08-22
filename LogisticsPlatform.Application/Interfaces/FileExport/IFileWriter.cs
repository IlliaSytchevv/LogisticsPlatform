namespace LogisticsPlatform.Application.Interfaces.FileExport;

public interface IFileWriter
{
    Task WriteAsync<T>(
        Stream output,
        string title,
        IReadOnlyList<string> headers,
        IAsyncEnumerable<T> rows,
        Func<T, IReadOnlyList<object?>> mapRow,
        CancellationToken cancellationToken = default);
}
