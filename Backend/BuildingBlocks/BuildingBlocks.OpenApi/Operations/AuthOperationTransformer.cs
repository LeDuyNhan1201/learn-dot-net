using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BuildingBlocks.OpenApi.Operations;

public sealed class AuthOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken _)
    {
        var endpointMetadata = context.Description.ActionDescriptor.EndpointMetadata;

        if (endpointMetadata.OfType<IAllowAnonymous>().Any()) return Task.CompletedTask;

        var requiresAuth = endpointMetadata.OfType<IAuthorizeData>().Any();

        if (!requiresAuth) return Task.CompletedTask;

        operation.Security ??= [];

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SecuritySchemeType.Http.GetDisplayName(), context.Document)] = []
        });

        List<string> scopes = ["openid", "profile", "email"];

        // operation.Security.Add(new OpenApiSecurityRequirement
        // {
        //     [new OpenApiSecuritySchemeReference(SecuritySchemeType.OAuth2.GetDisplayName(), context.Document)] = scopes
        // });

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SecuritySchemeType.OpenIdConnect.GetDisplayName(), context.Document)] = scopes
        });

        return Task.CompletedTask;
    }
}