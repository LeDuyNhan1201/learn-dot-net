using BuildingBlocks.OpenApi.Abstractions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BuildingBlocks.OpenApi.Metadata;

public sealed class SchemaExampleTransformer(IEnumerable<ISchemaExampleProvider> providers) : IOpenApiSchemaTransformer
{
    private readonly Dictionary<Type, ISchemaExampleProvider> _providers = providers.ToDictionary(provider => provider.TargetType);

    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (_providers.TryGetValue(context.JsonTypeInfo.Type, out var provider))
        {
            schema.Example = provider.GetExample();
        }

        return Task.CompletedTask;
    }
}