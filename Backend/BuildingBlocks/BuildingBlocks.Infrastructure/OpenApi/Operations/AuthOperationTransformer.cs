using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BuildingBlocks.Infrastructure.OpenApi.Operations;

public sealed class AuthOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken _)
    {
        var requiresAuth = context.Description.ActionDescriptor
            .EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .Any();

        if (!requiresAuth) return Task.CompletedTask;

        operation.Security ??= [];

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SecuritySchemeType.Http.GetDisplayName(), context.Document)] = []
        });

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SecuritySchemeType.OAuth2.GetDisplayName(), context.Document)] =
            [
                "read",
                "write"
            ]
        });

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SecuritySchemeType.OpenIdConnect.GetDisplayName(), context.Document)] =
            [
                "openid",
                "profile",
                "email"
            ]
        });

        return Task.CompletedTask;
    }
}