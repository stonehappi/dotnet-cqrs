namespace DotnetCqrs.Infrastructure.Data;

public abstract class Entity;

public abstract class IdEntity : Entity
{
    public int Id { get; set; }
}

public abstract class AuditableEntity : IdEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public int? CreatedBy { get; set; }
    public int? DeletedBy { get; set; }
}