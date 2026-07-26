using System.ComponentModel;
using System.Text.Json.Serialization;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Application.DTOs;

public sealed record CreateMenuItemRequest
{
    [JsonPropertyName("name")] public string? MenuItemName { get; set; }
    [JsonPropertyName("description")] public string? MenuItemDescription { get; set; }
    [JsonPropertyName("imageUrl")] public string? MenuItemImageUrl { get; set; }
    [JsonPropertyName("price")] public decimal MenuItemPrice { get; set; }

    [DefaultValue(MenuCategory.Food)]
    [JsonPropertyName("category")]
    public MenuCategory Category { get; set; }

    [DefaultValue(MenuSubCategory.Breakfast)]
    [JsonPropertyName("subCategory")]
    public MenuSubCategory SubCategory { get; set; }
}