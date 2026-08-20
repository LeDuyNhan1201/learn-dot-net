using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Abstractions.Bussiness;
using BuildingBlocks.Domain.Enumerations;
using BuildingBlocks.Domain.Models;
using BuildingBlocks.SharedKernel.Utils;

namespace BuildingBlocks.Application.Services;

public sealed class UserSeederService(IKeycloakAdminClient keycloakAdminClient) : IUserSeederService
{
    public async Task<IReadOnlyCollection<string>> InitAdministrators(CancellationToken cancellationToken = default)
    {
        var administrators = new[]
        {
            new CreateKeycloakUserRequest(
                "admin",
                Constants.AdministratorSampleEmail,
                "Test",
                "Admin",
                Constants.AdministratorSamplePassword,
                true,
                true,
                null,
                KeycloakUserGroup.Administrators
            )
        };

        return await keycloakAdminClient.CreateUsersAsync(administrators, cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> InitCustomers(CancellationToken cancellationToken = default)
    {
        var customers = new[]
        {
            new CreateKeycloakUserRequest(
                "customer",
                Constants.CustomerSampleEmail,
                "Test",
                "Customer",
                Constants.CustomerSamplePassword,
                true,
                true,
                null,
                KeycloakUserGroup.Customers
            )
        };

        return await keycloakAdminClient.CreateUsersAsync(customers, cancellationToken);
    }
}