using BuildingBlocks.Application.RestClients;
using BuildingBlocks.SharedKernel.DTOs;
using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Identity.keycloakAdmin;

public sealed class KeycloakAdminClient(
    IOptions<KeycloakAdminClientOptions> options,
    IKeycloakUserClient userResources)
    : IKeycloakAdminClient
{
    private readonly string _realm = options.Value.Realm;

    public async Task<Guid?> CreateUserAsync(
        KeycloakModels.CreateUser request,
        CancellationToken cancellationToken = default)
    {
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

        await userResources.CreateUserAsync(_realm, user, cancellationToken);

        var parameters = new GetUsersRequestParameters
        {
            Username = request.Username,
            Exact = true
        };
        var created = await userResources.GetUsersAsync(_realm, parameters, cancellationToken);

        return Guid.TryParse(created.FirstOrDefault()?.Id, out var id) ? id : null;
    }

    public async Task<IReadOnlyCollection<Guid>> CreateUsersAsync(
        IEnumerable<KeycloakModels.CreateUser> requests,
        CancellationToken cancellationToken = default)
    {
        var ids = new List<Guid>();

        foreach (var request in requests)
        {
            var id = await CreateUserAsync(request, cancellationToken);
            if (id.HasValue) ids.Add(id.Value);
        }

        return ids;
    }

    public async Task<UserRepresentation?> GetUserAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await userResources.GetUserAsync(_realm,
            id.ToString(),
            true,
            cancellationToken);
    }

    public async Task<IEnumerable<UserRepresentation>> GetUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return await userResources.GetUsersAsync(_realm,
            cancellationToken: cancellationToken);
    }

    public Task DeleteUserAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return userResources.DeleteUserAsync(_realm,
            id.ToString(),
            cancellationToken);
    }

    public async Task DeleteUsersAsync(IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        foreach (var id in ids) await DeleteUserAsync(id, cancellationToken);
    }
}