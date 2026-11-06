using AlunosTech.Domain.Entities;

namespace AlunosTech.Application.Interfaces;

public interface IAlunoRepository
{
    Task<Aluno?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Aluno aluno, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Aluno>> GetAllAsync(CancellationToken cancellationToken = default);
}
