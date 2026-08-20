using BuildingBlocks.Domain.Models;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;

namespace BuildingBlocks.Application.Abstractions;

public interface IKeycloakAdminClient
{
    Task<KeycloakTokenResponse> GetTokensAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    Task<string?> CreateUserAsync(
        CreateKeycloakUserRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> CreateUsersAsync(
        IEnumerable<CreateKeycloakUserRequest> requests,
        CancellationToken cancellationToken = default);

    Task<UserRepresentation?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<UserRepresentation?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserRepresentation>> GetUsersAsync(
        GetUsersRequestParameters? parameters = null,
        CancellationToken cancellationToken = default);

    Task DeleteUserAsync(string id, CancellationToken cancellationToken = default);

    Task DeleteUsersAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}