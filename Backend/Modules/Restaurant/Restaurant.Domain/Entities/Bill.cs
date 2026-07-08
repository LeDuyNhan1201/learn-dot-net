using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Entities;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Domain.Entities;

[Table("bills")]
public class Bill : AuditEntity<Guid>
{
    [MaxLength(50)]
    [Column("bill_number")]
    public string BillNumber { get; set; } = null!;

    [Column("bill_date")] public DateTime BillDate { get; set; }

    [Column("ordered_time")] public DateTime OrderedTime { get; set; }

    [Column("total_amount")] public BillStatus Status { get; set; }

    [Column("total_price")] public decimal TotalPrice { get; set; }

    public virtual ICollection<BillItem> Items { get; set; } = [];
}