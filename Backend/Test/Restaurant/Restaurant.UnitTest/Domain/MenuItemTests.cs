using FluentAssertions;
using Restaurant.Domain.Contracts.DomainEvents;
using Restaurant.Domain.Entities;
using Restaurant.Testing.Factories;
using Xunit;

namespace Restaurant.UnitTest.Domain;

public class MenuItemTests
{
    [Fact]
    public void Should_Create_MenuItem()
    {
        // Arrange
        var request = MenuItemFactory.ValidCreateRequest;
        var command = MenuItemFactory.ValidCreateCommand;

        // Act
        var menuItem = MenuItem.Create(command);

        // Assert
        menuItem.Name.Should().Be(request.MenuItemName);
        menuItem.Price.Should().Be(request.MenuItemPrice);
        menuItem.Category.Should().Be(request.Category);
        menuItem.SubCategory.Should().Be(request.SubCategory);

        var domainEvent = menuItem.DomainEvents
            .Should()
            .ContainSingle(@event => @event is MenuItemCreatedDomainEvent)
            .Which;

        domainEvent.Should().BeOfType<MenuItemCreatedDomainEvent>();
    }
}