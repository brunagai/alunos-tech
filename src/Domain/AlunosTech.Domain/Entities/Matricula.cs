using AlunosTech.Domain.Common;
using AlunosTech.Domain.Exceptions;

namespace AlunosTech.Domain.Entities;

public class Matricula : BaseEntity
{
    public Guid AlunoId { get; private set; }

    public Guid CursoId { get; private set; }

    public StatusMatricula Status { get; private set; }

    public DateTime DataMatricula { get; private set; }

    public Aluno Aluno { get; private set; } = null!;

    public Curso Curso { get; private set; } = null!;

    private Matricula()
    {
    }

    public Matricula(Aluno aluno, Curso curso)
    {
        ArgumentNullException.ThrowIfNull(aluno);
        ArgumentNullException.ThrowIfNull(curso);

        AlunoId = Guard.Obrigatorio(aluno.Id, nameof(AlunoId));
        CursoId = Guard.Obrigatorio(curso.Id, nameof(CursoId));
        Status = StatusMatricula.Ativa;
        DataMatricula = DateTime.UtcNow;
        Aluno = aluno;
        Curso = curso;

        aluno.AdicionarMatricula(this);
        curso.AdicionarMatricula(this);
    }

    public void AlterarStatus(StatusMatricula status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new DomainException("Status da matrícula é inválido.");
        }

        Status = status;
        MarcarAtualizacao();
    }
}
