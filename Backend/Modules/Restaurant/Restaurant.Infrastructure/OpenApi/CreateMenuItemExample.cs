using System.Text.Json.Nodes;
using BuildingBlocks.OpenApi.Abstractions;
using Restaurant.Application.DTOs;

namespace Restaurant.Infrastructure.OpenApi;

public sealed class CreateMenuItemExample : ISchemaExampleProvider
{
    public Type TargetType => typeof(CreateMenuItemRequest);

    public JsonNode GetExample()
    {
        return new JsonObject
        {
            ["name"] = "Fried Chicken",
            ["description"] = "Crispy fried chicken",
            ["imageUrl"] = "https://example.com/chicken.jpg",
            ["price"] = 9.99m,
            ["category"] = "Food",
            ["subCategory"] = "Dinner"
        };
    }
}