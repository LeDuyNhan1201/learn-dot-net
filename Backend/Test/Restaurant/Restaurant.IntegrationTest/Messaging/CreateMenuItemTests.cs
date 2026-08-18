using FluentAssertions;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Consumers;
using Restaurant.Domain.Contracts.IntegrationEvents;
using Restaurant.Testing.Fixtures;
using Restaurant.Testing.Integration;
using Xunit;

namespace Restaurant.IntegrationTest.Messaging;

[Collection("RestaurantIntegrationTest")]
public class CreateMenuItemTests(RestaurantFixture fixture) : MenuItemHttpEndpointTest(fixture)
{
    [Fact]
    public async Task Should_Publish_Integration_Event()
    {
        // Arrange
        var harness = Services.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var accessToken = Fixture.Factory.AdminAccessToken;
        await PostAsync(MenuItemUri, ValidRequest, accessToken);

        // Assert execution
        var isEventPublished = await harness.Published.Any<MenuItemCreatedIntegrationEvent>
            (TestContext.Current.CancellationToken);

        isEventPublished.Should().BeTrue();

        await harness.Stop(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Should_Consume_Integration_Event()
    {
        // Arrange
        var harness = Services.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var accessToken = Fixture.Factory.AdminAccessToken;
        await PostAsync(MenuItemUri, ValidRequest, accessToken);

        // Assert execution
        var consumerHarness = harness.GetConsumerHarness<MenuItemCreatedConsumer>();

        var isEventConsumed = await consumerHarness.Consumed.Any<MenuItemCreatedIntegrationEvent>
            (TestContext.Current.CancellationToken);

        isEventConsumed.Should().BeTrue();

        await harness.Stop(TestContext.Current.CancellationToken);
    }
}