using FluentAssertions;
using Restaurant.Domain.Contracts;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enumerations;

namespace Restaurant.UnitTest.Domain;

public class MenuItemTests
{
    [Fact]
    public void Create_Should_Create_MenuItem()
    {
        // Arrange
        var command = new CreateMenuItemCommand
        {
            MenuItemName = "Chicken",
            MenuItemDescription = "Crispy",
            ImageUrl = "abc",
            MenuItemPrice = 10,
            Category = MenuCategory.Food,
            SubCategory = MenuSubCategory.Dinner
        };

        // Act
        var menuItem = MenuItem.Create(command);

        // Assert
        menuItem.Name.Should().Be("Chicken");
        menuItem.Price.Should().Be(10);
        menuItem.Category.Should().Be(MenuCategory.Food);

        var domainEvent = menuItem.DomainEvents
            .Should()
            .ContainSingle(@event => @event is MenuItemCreatedDomainEvent)
            .Which;

        domainEvent.Should().BeOfType<MenuItemCreatedDomainEvent>();
    }
}