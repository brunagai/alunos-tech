using AlunosTech.Domain.Entities;

namespace AlunosTech.Application.Interfaces;

public interface ICursoRepository
{
    Task<Curso?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Curso curso, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Curso>> GetAllAsync(CancellationToken cancellationToken = default);
}
