using System.Text.Json.Serialization;

namespace Restaurant.Application.DTOs;

public interface IMenuItemDto
{
    public sealed record CreateRequest
    {
        [JsonPropertyName("name")] public string? MenuItemName { get; init; }

        [JsonPropertyName("description")] public string? MenuItemDescription { get; init; }

        [JsonPropertyName("imageUrl")] public string? MenuItemImageUrl { get; init; }

        [JsonPropertyName("price")] public decimal MenuItemPrice { get; init; }
    }
}