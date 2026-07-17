using System.Security.Claims;
using Keycloak.AuthServices.Common;

namespace BuildingBlocks.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
    private const string SubjectClaimType = "sub";
    private const string EmailClaimType = "email";

    public static string GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return principal.FindFirst(SubjectClaimType)?.Value
               ?? throw new InvalidOperationException("Missing sub claim.");
    }

    public static string GetUsername(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return principal.FindFirst(KeycloakConstants.NameClaimType)?.Value
               ?? throw new InvalidOperationException($"Missing {KeycloakConstants.NameClaimType} claim.");
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return principal.FindFirst(EmailClaimType)?.Value;
    }
}