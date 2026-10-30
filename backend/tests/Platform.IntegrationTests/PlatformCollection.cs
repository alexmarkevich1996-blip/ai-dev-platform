namespace Platform.IntegrationTests;

/// <summary>
/// Shares one container and one composed application across every test class, so
/// the suite starts PostgreSQL once rather than once per class.
/// </summary>
[CollectionDefinition(Name)]
public sealed class PlatformCollection : ICollectionFixture<PlatformApplication>
{
    public const string Name = "Platform application";
}
