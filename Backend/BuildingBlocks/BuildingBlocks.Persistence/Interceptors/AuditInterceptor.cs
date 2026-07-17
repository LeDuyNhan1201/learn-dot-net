using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.ExecutionContext.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.Interceptors;

public sealed class AuditInterceptor(IExecutionContextAccessor executionContextAccessor) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null) ApplyAudit(eventData.Context);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void ApplyAudit(DbContext context)
    {
        var actor = executionContextAccessor.Current.UserId;

        var now = DateTimeOffset.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditEntity entity)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:

                    entity.CreatedAt = now;
                    entity.CreatedBy = actor;

                    break;

                case EntityState.Modified:

                    entry.Property(nameof(entity.CreatedAt))
                        .IsModified = false;

                    entry.Property(nameof(entity.CreatedBy))
                        .IsModified = false;

                    if (entity.IsDeleted)
                    {
                        entity.DeletedAt = now;
                        entity.DeletedBy = actor;
                    }
                    else
                    {
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = actor;
                    }

                    break;
            }
        }
    }
}