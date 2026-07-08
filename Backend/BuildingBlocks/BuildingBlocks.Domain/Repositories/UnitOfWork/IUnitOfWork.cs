using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.Domain.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default);
}
