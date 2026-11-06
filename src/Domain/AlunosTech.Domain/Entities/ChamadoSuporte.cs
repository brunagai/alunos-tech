using AlunosTech.Domain.Common;
using AlunosTech.Domain.Exceptions;

namespace AlunosTech.Domain.Entities;

public class ChamadoSuporte : BaseEntity
{
    public const int TituloMaxLength = 150;
    public const int DescricaoMaxLength = 2000;

    public Guid AlunoId { get; private set; }

    public string Titulo { get; private set; } = null!;

    public string? Descricao { get; private set; }

    public StatusChamado Status { get; private set; }

    public PrioridadeChamado Prioridade { get; private set; }

    public Aluno Aluno { get; private set; } = null!;

    private ChamadoSuporte()
    {
    }

    public ChamadoSuporte(Aluno aluno, string titulo, PrioridadeChamado prioridade, string? descricao = null)
    {
        ArgumentNullException.ThrowIfNull(aluno);
        ValidarPrioridade(prioridade);

        AlunoId = Guard.Obrigatorio(aluno.Id, nameof(AlunoId));
        Titulo = Guard.Obrigatorio(titulo, TituloMaxLength, nameof(Titulo));
        Descricao = Guard.Opcional(descricao, DescricaoMaxLength, nameof(Descricao));
        Status = StatusChamado.Aberto;
        Prioridade = prioridade;
        Aluno = aluno;

        aluno.AdicionarChamado(this);
    }

    public void AlterarStatus(StatusChamado status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new DomainException("Status do chamado é inválido.");
        }

        Status = status;
        MarcarAtualizacao();
    }

    public void AlterarPrioridade(PrioridadeChamado prioridade)
    {
        ValidarPrioridade(prioridade);
        Prioridade = prioridade;
        MarcarAtualizacao();
    }

    private static void ValidarPrioridade(PrioridadeChamado prioridade)
    {
        if (!Enum.IsDefined(prioridade))
        {
            throw new DomainException("Prioridade do chamado é inválida.");
        }
    }
}
