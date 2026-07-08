using BuildingBlocks.Shared.DTOs;
using Keycloak.AuthServices.Sdk.Admin.Models;

namespace BuildingBlocks.Application.RestClients;

public interface IKeycloakAdminClient
{
    Task<Guid?> CreateUserAsync(
        KeycloakModels.CreateUser request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Guid>> CreateUsersAsync(
        IEnumerable<KeycloakModels.CreateUser> requests,
        CancellationToken cancellationToken = default);

    Task<UserRepresentation?> GetUserAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<UserRepresentation>> GetUsersAsync(
        CancellationToken cancellationToken = default);

    Task DeleteUserAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task DeleteUsersAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}