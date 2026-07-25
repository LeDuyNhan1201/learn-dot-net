using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Restaurant.Application.DTOs;

namespace Restaurant.Infrastructure.OpenApi;

public sealed class SampleSchemaOperationTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(CreateMenuItemRequest))
            return Task.CompletedTask;

        schema.Example = new JsonObject
        {
            ["name"] = "Fried Chicken",
            ["description"] = "Crispy fried chicken",
            ["imageUrl"] = "https://example.com/chicken.jpg",
            ["price"] = 9.99m,
            ["category"] = "Food",
            ["subCategory"] = "Dinner"
        };

        return Task.CompletedTask;
    }
}