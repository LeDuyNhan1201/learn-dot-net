using BuildingBlocks.Application.RestClients;
using BuildingBlocks.Infrastructure.Authentication.keycloakAdmin.Options;
using Duende.AccessTokenManagement;
using Keycloak.AuthServices.Sdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Authentication.keycloakAdmin.Extensions;

public static class KeycloakAdminExtensions
{
    public static readonly string TokenClientName = "keycloak-admin";
    
    public static IServiceCollection AddKeycloakAdmin(
        this IServiceCollection services,
        KeycloakAdminClientOptions options)
    {
        var tokenClientName = ClientCredentialsClientName.Parse(TokenClientName);

        services.AddDistributedMemoryCache();

        services
            .AddClientCredentialsTokenManagement()
            .AddClient(tokenClientName, client =>
            {
                client.ClientId = ClientId.Parse(options.Resource);
                client.ClientSecret = ClientSecret.Parse(options.Credentials.Secret);
                client.TokenEndpoint = new Uri(options.KeycloakTokenEndpoint);
            });

        services
            .AddKeycloakAdminHttpClient(options)
            .AddClientCredentialsTokenHandler(tokenClientName);
        
        services.AddScoped<IKeycloakAdminClient, KeycloakAdminClient>();

        return services;
    }
    
    public static IServiceCollection AddKeycloakAdmin(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetRequiredSection(KeycloakAdminOptions.Section)
            .Get<KeycloakAdminClientOptions>()!;
        
        return AddKeycloakAdmin(services, options);
    }
}