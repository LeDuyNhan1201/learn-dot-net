using System.Net;
using BuildingBlocks.SharedKernel.DTOs;
using BuildingBlocks.SharedKernel.Errors.Models;
using BuildingBlocks.Testing.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.DTOs;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Testing.Fixtures;
using Restaurant.Testing.Integration;
using Xunit;

namespace Restaurant.IntegrationTest.APIs.v2;

[Collection("RestaurantIntegrationTest")]
public class CreateMenuItemTests(RestaurantFixture fixture) : MenuItemHttpEndpointTest(fixture), IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        // BEFORE EACH
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        // AFTER EACH
        return ValueTask.CompletedTask;
    }
    
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
    public async Task Should_Return_401_When_AccessToken_Invalid()
    {
        // Arrange
        var request = ValidRequest;
        var response = await PostAsync(MenuItemUri, request);

        // Assert response
        await response.AssertResponseAsync<BaseResponse<object>>(
            HttpStatusCode.Unauthorized, 
            AuthErrors.Unauthorized.Code);
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
            menuItemRequest.MenuItemPrice, ValidationErrors.Valid.MessageKey);
    }
}