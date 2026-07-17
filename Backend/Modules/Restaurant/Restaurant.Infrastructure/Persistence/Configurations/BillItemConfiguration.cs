using BuildingBlocks.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public sealed class BillItemConfiguration : AuditEntityConfiguration<BillItem>
{
    protected override void ConfigureKey(EntityTypeBuilder<BillItem> builder)
    {
        builder.HasKey(x => new
        {
            x.BillId,
            x.MenuItemId
        });
    }

    protected override void ConfigureEntity(EntityTypeBuilder<BillItem> builder)
    {
        builder.ToTable("bill_items", table =>
        {
            table.HasCheckConstraint(
                "ck_bill_item_quantity",
                "\"quantity\" > 0");

            table.HasCheckConstraint(
                "ck_bill_item_unit_price",
                "\"unit_price\" >= 0");

            table.HasCheckConstraint(
                "ck_bill_item_total_price",
                "\"total_price\" >= 0");
        });

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(x => x.Bill)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.BillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MenuItem)
            .WithMany(x => x.BillItems)
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}