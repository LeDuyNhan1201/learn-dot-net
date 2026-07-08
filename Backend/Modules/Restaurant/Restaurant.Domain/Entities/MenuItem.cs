using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Entities;
using Restaurant.Domain.Enumerations;

namespace Restaurant.Domain.Entities;

[Table("menu_items")]
public class MenuItem : AuditEntity<Guid>
{
    [MaxLength(500)] [Column("name")] public string Name { get; set; } = null!;

    [MaxLength(1000)]
    [Column("description")]
    public string Description { get; set; } = null!;

    [MaxLength(2000)]
    [Column("additional_details")]
    public string? AdditionalDetails { get; set; }

    [MaxLength(500)] [Column("image_url")] public string? ImageUrl { get; set; }

    [Column("price")] public decimal Price { get; set; }

    [MaxLength(500)] [Column("note")] public string? Note { get; set; }

    [Column("category")] public MenuCategory Category { get; set; }

    [Column("sub_category")] public MenuSubCategory SubCategory { get; set; }

    public virtual ICollection<BillItem> BillItems { get; set; } = [];
}