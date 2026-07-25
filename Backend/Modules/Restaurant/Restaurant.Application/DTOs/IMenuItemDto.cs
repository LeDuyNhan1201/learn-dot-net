using System.ComponentModel;
using System.Text.Json.Serialization;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Application.DTOs;

public interface IMenuItemDto
{
    public sealed record CreateRequest
    {
        [JsonPropertyName("name")] public string? MenuItemName { get; init; }
        [JsonPropertyName("description")] public string? MenuItemDescription { get; init; }
        [JsonPropertyName("imageUrl")] public string? MenuItemImageUrl { get; init; }
        [JsonPropertyName("price")] public decimal MenuItemPrice { get; init; }
        [DefaultValue(MenuCategory.Food)]
        [JsonPropertyName("category")] public MenuCategory Category { get; init; }
        [DefaultValue(MenuSubCategory.Breakfast)]
        [JsonPropertyName("subCategory")] public MenuSubCategory SubCategory { get; init; }
    }
}