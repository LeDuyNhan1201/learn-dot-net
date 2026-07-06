using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingBlocks.Domain.Entities;

public abstract class AuditEntity : IAuditEntity
{
    [Column("created_by")] public string? CreatedBy { get; set; } = "system";

    [Column("created_at")] public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_by")] public string? UpdatedBy { get; set; }

    [Column("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }

    [Column("deleted_by")] public string? DeletedBy { get; set; }

    [Column("deleted_at")] public DateTimeOffset? DeletedAt { get; set; }

    [Column("is_deleted")] public bool IsDeleted { get; set; }
}

public abstract class AuditEntity<TId> : AuditEntity
    where TId : notnull
{
    [Key] [Column("id")] public required TId Id { get; set; }
}

public interface IAuditEntity
{
    string? CreatedBy { get; set; }
    DateTimeOffset CreatedAt { get; set; }

    string? UpdatedBy { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }

    string? DeletedBy { get; set; }
    DateTimeOffset? DeletedAt { get; set; }

    bool IsDeleted { get; set; }
}