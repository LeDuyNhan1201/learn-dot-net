using System;
using BuildingBlocks.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class BillConfiguration : AuditEntityConfiguration<Bill, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Bill> builder)
    {
        builder.ToTable("bills");

        builder.Property(x => x.BillNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.BillNumber)
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.OrderedTime)
            .IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Bill)
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}