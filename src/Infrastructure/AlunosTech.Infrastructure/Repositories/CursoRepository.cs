using AlunosTech.Application.Interfaces;
using AlunosTech.Domain.Entities;
using AlunosTech.Infrastructure.Context;

namespace AlunosTech.Infrastructure.Repositories;

public class CursoRepository : RepositoryBase<Curso>, ICursoRepository
{
    public CursoRepository(AlunosTechDbContext context)
        : base(context)
    {
    }
}
