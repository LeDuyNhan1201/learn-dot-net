using BuildingBlocks.Application.RestClients;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.SharedKernel.DTOs;

namespace BuildingBlocks.Application.Services;

public sealed class UserSeederService(IKeycloakAdminClient keycloakAdminClient) : IUserSeederService
{
    public async Task<IReadOnlyCollection<string>> InitAdministrators(CancellationToken cancellationToken = default)
    {
        var administrators = new[]
        {
            new CreateKeycloakUserRequest(
                Username: "admin",
                Email: "admin@test.com",
                FirstName: "Test",
                LastName: "Admin",
                Password: "Admin@123",
                Enabled: true,
                EmailVerified: true,
                Attributes: null,
                Roles: ["admin"]
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
                Email: "customer@test.com",
                FirstName: "Test",
                LastName: "Customer",
                Password: "Customer@123",
                Enabled: true,
                EmailVerified: true,
                Attributes: null,
                Roles: ["customer"]
            )
        };

        return await keycloakAdminClient.CreateUsersAsync(customers, cancellationToken);
    }
}