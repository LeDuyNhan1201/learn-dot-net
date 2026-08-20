using System.Net;
using System.Net.Http.Json;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Models;
using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Groups;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Identity.Clients;

public sealed class KeycloakAdminClient(
    IOptions<KeycloakAdminClientOptions> options,
    HttpClient httpClient,
    IKeycloakClient keycloak)
    : IKeycloakAdminClient
{
    private readonly string _realm = options.Value.Realm;

    public async Task<KeycloakTokenResponse> GetTokensAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var realm = options.Value.Realm;
        var clientId = options.Value.Resource;
        var clientSecret = options.Value.Credentials.Secret;

        var values = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["username"] = username,
            ["password"] = password
        };

        if (!string.IsNullOrEmpty(clientSecret)) values["client_secret"] = clientSecret;

        using var content = new FormUrlEncodedContent(values);

        var response = await httpClient.PostAsync($"/realms/{realm}/protocol/openid-connect/token", content, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Keycloak token request failed. " +
                $"Status: {(int)response.StatusCode} {response.StatusCode}. " +
                $"Response: {responseBody}");

        var tokens = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);

        return tokens ?? throw new InvalidOperationException("Failed to retrieve access token.");
    }

    public async Task<string?> CreateUserAsync(
        CreateKeycloakUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var groupParameters = new GetGroupsRequestParameters
        {
            Search = request.Group.ToString(),
            Exact = true,
            BriefRepresentation = true
        };
        var groups = (await keycloak.GetGroupsAsync(_realm, groupParameters, cancellationToken))?.ToList();

        if (groups is null || groups.Count == 0) throw new InvalidOperationException($"Failed to retrieve Keycloak groups. Group '{request.Group}' not found.");

        var groupId = groups.First().Id ?? throw new InvalidOperationException($"Group '{request.Group}' does not have a valid ID.");

        var user = new UserRepresentation
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Enabled = request.Enabled,
            EmailVerified = request.EmailVerified,
            Attributes = request.Attributes,
            Credentials =
            [
                new CredentialRepresentation
                {
                    Type = "password",
                    Temporary = false,
                    Value = request.Password
                }
            ]
        };

        using var response = await keycloak.CreateUserWithResponseAsync(_realm, user, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Created) throw new InvalidOperationException($"Failed to create Keycloak user. " + $"StatusCode: {(int)response.StatusCode}.");

        var location = response.Headers.Location ?? throw new InvalidOperationException("Keycloak did not return Location header.");
        var userId = location.Segments[^1];

        await keycloak.JoinGroupWithResponseAsync(_realm, userId, groupId, cancellationToken);

        return userId;
    }

    public async Task<IReadOnlyCollection<string>> CreateUsersAsync(
        IEnumerable<CreateKeycloakUserRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var ids = new List<string>();

        foreach (var request in requests)
        {
            var id = await CreateUserAsync(request, cancellationToken);
            if (id != null) ids.Add(id);
        }

        return ids;
    }

    public async Task<UserRepresentation?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await keycloak.GetUserAsync(_realm, id, true, cancellationToken);
    }

    public async Task<UserRepresentation?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var parameters = new GetUsersRequestParameters
        {
            Email = email
        };

        var users = await keycloak.GetUsersAsync(_realm, parameters, cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task<IEnumerable<UserRepresentation>> GetUsersAsync(
        GetUsersRequestParameters? parameters = null,
        CancellationToken cancellationToken = default)
    {
        return await keycloak.GetUsersAsync(_realm, parameters, cancellationToken);
    }

    public Task DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        return keycloak.DeleteUserAsync(_realm, id, cancellationToken);
    }

    public async Task DeleteUsersAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids) await DeleteUserAsync(id, cancellationToken);
    }
}