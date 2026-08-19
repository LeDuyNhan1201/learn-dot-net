using BuildingBlocks.Application.RestClients;
using BuildingBlocks.Domain.Enumerations;
using BuildingBlocks.Domain.Models;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.SharedKernel.Helpers;

namespace BuildingBlocks.Application.Services;

public sealed class UserSeederService(IKeycloakAdminClient keycloakAdminClient) : IUserSeederService
{
    public async Task<IReadOnlyCollection<string>> InitAdministrators(CancellationToken cancellationToken = default)
    {
        var administrators = new[]
        {
            new CreateKeycloakUserRequest(
                Username: "admin",
                Email: Constants.AdministratorSampleEmail,
                FirstName: "Test",
                LastName: "Admin",
                Password: Constants.AdministratorSamplePassword,
                Enabled: true,
                EmailVerified: true,
                Attributes: null,
                Group: KeycloakUserGroup.Administrators
            )
        };

        return await keycloakAdminClient.CreateUsersAsync(administrators, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<string>> InitCustomers(CancellationToken cancellationToken = default)
    {
        var customers = new[]
        {
            new CreateKeycloakUserRequest(
                Username: "customer",
                Email: Constants.CustomerSampleEmail,
                FirstName: "Test",
                LastName: "Customer",
                Password: Constants.CustomerSamplePassword,
                Enabled: true,
                EmailVerified: true,
                Attributes: null,
                Group: KeycloakUserGroup.Customers
            )
        };

        return await keycloakAdminClient.CreateUsersAsync(customers, cancellationToken);
    }
}