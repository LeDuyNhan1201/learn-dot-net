using System;
using BuildingBlocks.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public sealed class MenuItemConfiguration : AuditEntityConfiguration<MenuItem, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.AdditionalDetails)
            .HasMaxLength(2000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Note)
            .HasMaxLength(500);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasConversion<int>();

        builder.Property(x => x.SubCategory)
            .HasConversion<int>();

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => new
        {
            x.Category,
            x.SubCategory
        });

        builder.HasMany(x => x.BillItems)
            .WithOne(x => x.MenuItem)
            .HasForeignKey(x => x.BillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}