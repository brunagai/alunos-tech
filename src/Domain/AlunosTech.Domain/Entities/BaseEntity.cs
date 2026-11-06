namespace AlunosTech.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        var agora = DateTime.UtcNow;
        CreatedAt = agora;
        UpdatedAt = agora;
    }

    protected void MarcarAtualizacao()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
