using AlunosTech.Domain.Common;

namespace AlunosTech.Domain.Entities;

public class Curso : BaseEntity
{
    public const int TituloMaxLength = 200;
    public const int DescricaoMaxLength = 1000;

    private readonly List<Matricula> _matriculas = [];

    public string Titulo { get; private set; } = null!;

    public string? Descricao { get; private set; }

    public int CargaHoraria { get; private set; }

    public bool Ativo { get; private set; }

    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    private Curso()
    {
    }

    public Curso(string titulo, int cargaHoraria, string? descricao = null)
    {
        Titulo = Guard.Obrigatorio(titulo, TituloMaxLength, nameof(Titulo));
        Descricao = Guard.Opcional(descricao, DescricaoMaxLength, nameof(Descricao));
        CargaHoraria = Guard.Positivo(cargaHoraria, nameof(CargaHoraria));
        Ativo = true;
    }

    public void AtualizarDados(string titulo, int cargaHoraria, string? descricao)
    {
        Titulo = Guard.Obrigatorio(titulo, TituloMaxLength, nameof(Titulo));
        Descricao = Guard.Opcional(descricao, DescricaoMaxLength, nameof(Descricao));
        CargaHoraria = Guard.Positivo(cargaHoraria, nameof(CargaHoraria));
        MarcarAtualizacao();
    }

    public void Ativar()
    {
        Ativo = true;
        MarcarAtualizacao();
    }

    public void Desativar()
    {
        Ativo = false;
        MarcarAtualizacao();
    }

    internal void AdicionarMatricula(Matricula matricula)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        _matriculas.Add(matricula);
    }
}
