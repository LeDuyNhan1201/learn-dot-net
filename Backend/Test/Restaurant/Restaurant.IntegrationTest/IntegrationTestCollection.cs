using BuildingBlocks.Testing.Fixtures;

namespace Restaurant.IntegrationTest;

[CollectionDefinition("RestaurantIntegrationTest")]
public sealed class IntegrationTestCollection : ICollectionFixture<PostgreSqlFixture>;