using BuildingBlocks.Application.RestClients;
using BuildingBlocks.Identity.Options;
using Duende.AccessTokenManagement;
using Keycloak.AuthServices.Sdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Identity.Extensions;

public static class KeycloakAdminExtensions
{
    public static readonly string TokenClientName = "keycloak-admin";

    public static IServiceCollection AddKeycloakAdmin(
        this IServiceCollection services,
        KeycloakAdminClientOptions options,
        IHostEnvironment environment)
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

        var keycloakAdminHttpClient = services.AddKeycloakAdminHttpClient(options);
        var adminClient = services.AddHttpClient<KeycloakAdminClient>(client =>
        {
            client.BaseAddress = new Uri(options.AuthServerUrl ?? throw new InvalidOperationException("AuthServerUrl is not configured."));
        });

        if (environment.IsEnvironment("Local"))
        {
            services
                .AddHttpClient(ClientCredentialsTokenManagementDefaults.BackChannelHttpClientName)
                .ConfigurePrimaryHttpMessageHandler(CreateIgnoreSslHandler);
            keycloakAdminHttpClient.ConfigurePrimaryHttpMessageHandler(CreateIgnoreSslHandler);
            adminClient.ConfigurePrimaryHttpMessageHandler(CreateIgnoreSslHandler);
        }

        keycloakAdminHttpClient.AddClientCredentialsTokenHandler(tokenClientName);

        services.AddScoped<IKeycloakAdminClient, KeycloakAdminClient>();

        return services;
    }

    private static HttpClientHandler CreateIgnoreSslHandler()
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    }

    public static IServiceCollection AddKeycloakAdmin(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var options = configuration
            .GetRequiredSection(KeycloakAdminOptions.Section)
            .Get<KeycloakAdminClientOptions>()!;

        return services.AddKeycloakAdmin(options, environment);
    }
}