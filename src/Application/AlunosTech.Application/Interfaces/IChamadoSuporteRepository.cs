using AlunosTech.Domain.Entities;

namespace AlunosTech.Application.Interfaces;

public interface IChamadoSuporteRepository
{
    Task<ChamadoSuporte?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(ChamadoSuporte chamado, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChamadoSuporte>> GetAllAsync(CancellationToken cancellationToken = default);
}
