using BuildingBlocks.Infrastructure.OpenApi.Options;
using Keycloak.AuthServices.Common;

namespace BuildingBlocks.Infrastructure.OpenApi.Utils;

internal static class KeycloakEndpointUrls
{
    private const string AuthorizationEndpointPath = "protocol/openid-connect/auth";

    internal static Uri GetAuthorizationEndpoint(ApiDocsOptions.KeycloakOptions options)
    {
        return GetRealmEndpoint(options, AuthorizationEndpointPath);
    }

    internal static Uri GetTokenEndpoint(ApiDocsOptions.KeycloakOptions options)
    {
        return GetRealmEndpoint(options, KeycloakConstants.TokenEndpointPath);
    }

    internal static Uri GetOpenIdConfigurationEndpoint(ApiDocsOptions.KeycloakOptions options)
    {
        return GetRealmEndpoint(options, KeycloakConstants.OpenIdConnectConfigurationPath);
    }

    private static Uri GetRealmEndpoint(ApiDocsOptions.KeycloakOptions options, string endpointPath)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.AuthServerUrl)) throw new InvalidOperationException("Keycloak AuthServerUrl is required.");

        if (string.IsNullOrWhiteSpace(options.Realm)) throw new InvalidOperationException("Keycloak Realm is required.");

        var realmBaseUrl = $"{options.AuthServerUrl.TrimEnd('/')}/realms/{Uri.EscapeDataString(options.Realm)}/";

        return new Uri(new Uri(realmBaseUrl, UriKind.Absolute), endpointPath.TrimStart('/'));
    }
}