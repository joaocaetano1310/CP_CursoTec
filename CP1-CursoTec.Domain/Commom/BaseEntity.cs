namespace CP1_CursoTec.Domain.Commom;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public bool Active { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public void Deactivate() => Active = false;
    
    public void Activate() => Active = true;
}