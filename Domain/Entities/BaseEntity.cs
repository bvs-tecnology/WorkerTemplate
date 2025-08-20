namespace Domain.Entities;

public abstract class BaseEntity(Guid? id = null)
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
    public Guid? CreatedBy { get; protected set; } = id;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    
    protected void Update() => UpdatedAt = DateTime.UtcNow;
}