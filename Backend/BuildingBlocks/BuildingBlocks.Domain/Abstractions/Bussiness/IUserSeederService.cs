namespace BuildingBlocks.Domain.Abstractions.Bussiness;

public interface IUserSeederService
{
    Task<IReadOnlyCollection<string>> InitAdministrators(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> InitCustomers(CancellationToken cancellationToken = default);
}