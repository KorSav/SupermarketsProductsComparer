namespace InfrastructureIntegrationTests;

public static class Collections
{
    public const string Container1 = "Container 1 collection";
}

[CollectionDefinition(Collections.Container1)]
public class Container1Collection : ICollectionFixture<DbContainerFixture>;
