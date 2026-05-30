using System.Security.Claims;

namespace BuildingBlocks.Infrastructure.Authentication.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirst("sub")?.Value ?? throw new Exception("Missing sub claim");
    
    public static string GetUsername(this ClaimsPrincipal principal)
        => principal.FindFirst("preferred_username")?.Value 
               ?? throw new Exception("Missing preferred_username claim");

    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirst("email")?.Value;
}