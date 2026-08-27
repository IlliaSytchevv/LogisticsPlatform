namespace LogisticsPlatform.Domain.Options;

public sealed class PhotoStorageOptions
{
    public const string SectionName = "PhotoStorage";

    /// <summary>Azure Storage / Azurite connection string.</summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>Blob container name (created if missing).</summary>
    public string ContainerName { get; set; } = "photos";
}
