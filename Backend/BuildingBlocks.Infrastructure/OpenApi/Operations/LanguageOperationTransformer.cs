using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Operations;

public sealed class LanguageOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken _)
    {
        operation.Parameters ??= [];

        var alreadyExists = operation.Parameters.Any(p =>
            p.In == ParameterLocation.Header &&
            string.Equals(p.Name, "Accept-Language", StringComparison.OrdinalIgnoreCase));

        if (!alreadyExists)
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Preferred language for the response",
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Default = "en-US"
                },
                Examples = new Dictionary<string, IOpenApiExample>
                {
                    ["en-US"] = new OpenApiExample
                    {
                        Summary = "English (United States)",
                        Value = "en-US"
                    },
                    ["fr-FR"] = new OpenApiExample
                    {
                        Summary = "French (France)",
                        Value = "fr-FR"
                    },
                    ["es-ES"] = new OpenApiExample
                    {
                        Summary = "Spanish (Spain)",
                        Value = "es-ES"
                    }
                }
            });

        return Task.CompletedTask;
    }
}