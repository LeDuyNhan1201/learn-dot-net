using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Operations;

public sealed class MultiPartFileOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken _)
    {
        var formFileParams = context
            .Description
            .ParameterDescriptions
            .Where(apiParameterDescription =>
                apiParameterDescription.Type == typeof(IFormFile) ||
                apiParameterDescription.Type == typeof(IEnumerable<IFormFile>))
            .ToList();

        if (formFileParams.Count == 0) return Task.CompletedTask;

        var properties = formFileParams
            .ToDictionary<ApiParameterDescription, string, IOpenApiSchema>(
                apiParameterDescription => apiParameterDescription.Name,
                _ => new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "binary"
                });

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypeNames.Multipart.FormData] = new()
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = properties
                    }
                }
            }
        };

        return Task.CompletedTask;
    }
}