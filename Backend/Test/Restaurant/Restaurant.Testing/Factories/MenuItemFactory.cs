using Restaurant.Application.DTOs;
using Restaurant.Domain.Contracts.Commands;
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

    public static CreateMenuItemRequest InvalidPriceCreateRequest => ValidCreateRequest with
    {
        MenuItemPrice = -10,
    };
    
    public static CreateMenuItemCommand ValidCreateCommand => new()
    {
        MenuItemName = ValidCreateRequest.MenuItemName,
        MenuItemDescription = ValidCreateRequest.MenuItemDescription,
        ImageUrl = ValidCreateRequest.MenuItemImageUrl,
        MenuItemPrice = ValidCreateRequest.MenuItemPrice,
        Category = ValidCreateRequest.Category,
        SubCategory = ValidCreateRequest.SubCategory
    };

    public static CreateMenuItemCommand InvalidPriceCreateCommand => ValidCreateCommand with
    {
        MenuItemPrice = -10,
    };
}