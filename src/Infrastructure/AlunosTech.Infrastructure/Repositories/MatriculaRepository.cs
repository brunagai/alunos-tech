using AlunosTech.Application.Interfaces;
using AlunosTech.Domain.Entities;
using AlunosTech.Infrastructure.Context;

namespace AlunosTech.Infrastructure.Repositories;

public class MatriculaRepository : RepositoryBase<Matricula>, IMatriculaRepository
{
    public MatriculaRepository(AlunosTechDbContext context)
        : base(context)
    {
    }
}
