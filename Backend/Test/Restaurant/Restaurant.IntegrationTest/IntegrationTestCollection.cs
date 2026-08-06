using BuildingBlocks.Testing.Fixtures;
using Xunit;

namespace Restaurant.IntegrationTest;

[CollectionDefinition("RestaurantIntegrationTest")]
public sealed class IntegrationTestCollection : ICollectionFixture<PostgreSqlFixture>;