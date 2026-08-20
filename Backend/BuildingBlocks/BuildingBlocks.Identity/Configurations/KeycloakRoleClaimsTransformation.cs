using System.Net;
using System.Security.Claims;
using System.Text.Json;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.SharedKernel.Errors.Models;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Identity.Configurations;

public sealed class KeycloakRoleClaimsTransformation(IOptions<KeycloakAdminClientOptions> options) : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity { IsAuthenticated: true } identity) return Task.FromResult(principal);

        var resourceAccess = principal.FindFirst("resource_access");
        if (resourceAccess is null) return Task.FromResult(principal);

        using var document = JsonDocument.Parse(resourceAccess.Value);
        var clientId = options.Value.Resource;

        if (string.IsNullOrWhiteSpace(clientId)
            || !document.RootElement.TryGetProperty(clientId, out var client)
            || !client.TryGetProperty("roles", out var roles))
            return Task.FromResult(principal);

        foreach (var roleName in roles.EnumerateArray().Select(element => element.GetString()).Where(role => !string.IsNullOrWhiteSpace(role)))
            if (!identity.HasClaim(KeycloakConstants.RoleClaimType, roleName ?? throw new AppException((int)HttpStatusCode.Unauthorized, AuthErrors.Unauthorized)))
                identity.AddClaim(new Claim(KeycloakConstants.RoleClaimType, roleName));

        return Task.FromResult(principal);
    }
}