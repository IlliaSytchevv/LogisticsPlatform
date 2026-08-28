namespace LogisticsPlatform.Domain.Options;

public sealed class PhotoStorageOptions
{
    public const string SectionName = "PhotoStorage";
    
    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = "photos";
}
