using System.Security.Claims;
using Keycloak.AuthServices.Common;

namespace BuildingBlocks.Infrastructure.Authentication.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("sub")?.Value ?? throw new Exception("Missing sub claim");
    }

    public static string GetUsername(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(KeycloakConstants.NameClaimType)?.Value
               ?? throw new Exception("Missing preferred_username claim");
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("email")?.Value;
    }
}