using Restaurant.Application.DTOs;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Testing.Factories;

public static class MenuItemFactory
{
    public static CreateMenuItemRequest ValidCreateRequest => new()
    {
        MenuItemName = "Chicken",
        MenuItemDescription = "Crispy",
        MenuItemImageUrl = "abc",
        MenuItemPrice = 10,
        Category = MenuCategory.Food,
        SubCategory = MenuSubCategory.Dinner
    };

    public static CreateMenuItemRequest InvalidPriceRequest()
    {
        const decimal invalidPrice = -10;

        var request = ValidCreateRequest;
        request.MenuItemPrice = invalidPrice;

        return request;
    }
}