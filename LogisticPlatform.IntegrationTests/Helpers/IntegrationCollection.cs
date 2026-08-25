using LogisticPlatform.IntegrationTests.Fixtures;

namespace LogisticPlatform.IntegrationTests.Helpers;

[CollectionDefinition(Name)]
public sealed class IntegrationCollection : ICollectionFixture<LogisticsApiFixture>
{
    public const string Name = "LogisticsIntegration";
}