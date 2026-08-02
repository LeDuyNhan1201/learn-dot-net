using System.Net;
using BuildingBlocks.SharedKernel.Errors.Models;
using BuildingBlocks.Testing.Fixtures;
using BuildingBlocks.Testing.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.DTOs;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Testing.Integration;

namespace Restaurant.IntegrationTest.APIs.v2;

[Collection("RestaurantIntegrationTest")]
public class CreateMenuItemTests(PostgreSqlFixture postgres) : MenuItemHttpEndpointTest(postgres)
{
    [Fact]
    public async Task Should_Create_MenuItem()
    {
        // Arrange
        var request = ValidRequest;
        var response = await PostAsync(MenuItemUri, request);

        // Assert response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert database
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
        db.MenuItems.Should().Contain(menuItem => menuItem.Name == request.MenuItemName);
    }

    [Fact]
    public async Task Should_Return_400_When_Price_Invalid()
    {
        // Arrange
        var request = InvalidPriceRequest;
        var response = await PostAsync(MenuItemUri, request);

        // Assert response
        var body = await response.AssertValidationResponseAsync();

        body.ShouldHaveError<CreateMenuItemRequest>(menuItemRequest =>
            menuItemRequest.MenuItemPrice, ValidationErrors.Range.MessageKey);
    }
}