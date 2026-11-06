using AlunosTech.Application.Interfaces;
using AlunosTech.Domain.Entities;
using AlunosTech.Infrastructure.Context;

namespace AlunosTech.Infrastructure.Repositories;

public class ChamadoSuporteRepository : RepositoryBase<ChamadoSuporte>, IChamadoSuporteRepository
{
    public ChamadoSuporteRepository(AlunosTechDbContext context)
        : base(context)
    {
    }
}
