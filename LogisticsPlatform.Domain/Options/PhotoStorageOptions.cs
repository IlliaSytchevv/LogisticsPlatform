namespace LogisticsPlatform.Domain.Options;

public sealed class PhotoStorageOptions
{
    public const string SectionName = "PhotoStorage";

    public string RootPath { get; set; } = "App_Data/photos";
}