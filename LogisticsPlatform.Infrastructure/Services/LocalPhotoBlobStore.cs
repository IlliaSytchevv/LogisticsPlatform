using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LogisticsPlatform.Infrastructure.Services;

public sealed class LocalPhotoBlobStore : IPhotoBlobStore
{
    private readonly string _rootPath;

    public LocalPhotoBlobStore(IOptions<PhotoStorageOptions> options, IHostEnvironment environment)
    {
        string configured = options.Value.RootPath;
        _rootPath = Path.IsPathRooted(configured)
            ? configured
            : Path.GetFullPath(Path.Combine(environment.ContentRootPath, configured));

        Directory.CreateDirectory(_rootPath);
    }

    public async Task SaveAsync(string storageKey, byte[] content, CancellationToken cancellationToken)
    {
        string path = ResolvePath(storageKey);
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllBytesAsync(path, content, cancellationToken);
    }

    public Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken)
    {
        string path = ResolvePath(storageKey);
        if (!File.Exists(path))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 64 * 1024,
            options: FileOptions.Asynchronous | FileOptions.SequentialScan);

        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        string path = ResolvePath(storageKey);
        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }

    private string ResolvePath(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey)
            || storageKey.Contains("..", StringComparison.Ordinal)
            || Path.IsPathRooted(storageKey))
        {
            throw new ArgumentException("Invalid storage key.", nameof(storageKey));
        }

        string fullPath = Path.GetFullPath(Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar)));
        if (!fullPath.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Invalid storage key.", nameof(storageKey));

        return fullPath;
    }
}
