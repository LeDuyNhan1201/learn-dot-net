using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence.DbContexts;

public abstract class ApplicationDbContext<T>(DbContextOptions options) : DbContext(options)
    where T : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(T).Assembly);
    }
}