using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Swagger;

public sealed class AuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requiresAuth = context.ApiDescription.ActionDescriptor
            .EndpointMetadata.OfType<AuthorizeAttribute>().Any();

        if (!requiresAuth) return;

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference("Bearer", context.Document)
                ] = []
            }
        ];
    }
}