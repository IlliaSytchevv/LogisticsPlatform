using LogisticsPlatform.Application.Interfaces.Services;

namespace LogisticPlatform.IntegrationTests.Helpers;

public sealed class InMemoryPhotoBlobStore : IPhotoBlobStore
{
    private readonly Dictionary<string, byte[]> _blobs = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public Task SaveAsync(string storageKey, byte[] content, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            _blobs[storageKey] = content.ToArray();
        }

        return Task.CompletedTask;
    }

    public Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (!_blobs.TryGetValue(storageKey, out byte[]? bytes))
                return Task.FromResult<Stream?>(null);

            return Task.FromResult<Stream?>(new MemoryStream(bytes, writable: false));
        }
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            _blobs.Remove(storageKey);
        }

        return Task.CompletedTask;
    }
}