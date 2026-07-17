using System;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Entities;

namespace Restaurant.Domain.Entities;

[Table("bill_items")]
public class BillItem : AuditEntity
{
    [Column("bill_id")] public Guid BillId { get; set; }

    [Column("menu_item_id")] public Guid MenuItemId { get; set; }

    [Column("quantity")] public int Quantity { get; set; }

    [Column("unit_price")] public decimal UnitPrice { get; set; }

    [Column("total_price")] public decimal TotalPrice { get; set; }

    [ForeignKey("BillId")] public virtual Bill Bill { get; set; } = null!;

    [ForeignKey("MenuItemId")] public virtual MenuItem MenuItem { get; set; } = null!;
}