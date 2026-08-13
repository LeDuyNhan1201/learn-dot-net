using Restaurant.Testing.Fixtures;
using Xunit;

namespace Restaurant.IntegrationTest;

[CollectionDefinition("RestaurantIntegrationTest")]
public sealed class IntegrationTestCollection  : ICollectionFixture<RestaurantFixture>;