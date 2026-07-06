using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Persistence.Configurations;

public abstract class AuditEntityConfiguration<TEntity>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureKey(builder);
        ConfigureAudit(builder);
        ConfigureEntity(builder);
    }

    protected virtual void ConfigureKey(EntityTypeBuilder<TEntity> builder)
    {
        // Composite key entities override this.
    }

    protected virtual void ConfigureEntity(EntityTypeBuilder<TEntity> builder)
    {
    }

    protected virtual void ConfigureAudit(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.IsDeleted);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public abstract class AuditEntityConfiguration<TEntity, TId>
    : AuditEntityConfiguration<TEntity>
    where TEntity : AuditEntity<TId>
    where TId : notnull
{
    protected override void ConfigureKey(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
    }
}