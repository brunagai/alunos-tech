using AlunosTech.Domain.Entities;

namespace AlunosTech.Application.Interfaces;

public interface IMatriculaRepository
{
    Task<Matricula?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Matricula matricula, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Matricula>> GetAllAsync(CancellationToken cancellationToken = default);
}
