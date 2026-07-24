using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Entities;
using Restaurant.Domain.Contracts;
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
    
    public static MenuItem Create(IMenuItemCommand.Create command)
    {
        var entity = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = command.MenuItemName!,
            Description = command.MenuItemDescription!,
            ImageUrl = command.ImageUrl,
            Price = command.MenuItemPrice
        };
        
        var domainEvent = new IMenuItemDomainEvent.Created{
            Id = entity.Id.ToString(),
            MenuItemName = entity.Name,
            MenuItemDescription = entity.Description,
            ImageUrl = entity.ImageUrl,
            MenuItemPrice = entity.Price
        };

        entity.Raise(domainEvent);

        return entity;
    }
}