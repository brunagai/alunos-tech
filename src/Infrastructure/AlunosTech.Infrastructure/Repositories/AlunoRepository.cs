using AlunosTech.Application.Interfaces;
using AlunosTech.Domain.Entities;
using AlunosTech.Infrastructure.Context;

namespace AlunosTech.Infrastructure.Repositories;

public class AlunoRepository : RepositoryBase<Aluno>, IAlunoRepository
{
    public AlunoRepository(AlunosTechDbContext context)
        : base(context)
    {
    }
}
