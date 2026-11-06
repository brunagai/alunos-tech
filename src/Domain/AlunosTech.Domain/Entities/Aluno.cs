using AlunosTech.Domain.Common;
using AlunosTech.Domain.Exceptions;

namespace AlunosTech.Domain.Entities;

public class Aluno : BaseEntity
{
    public const int NomeMaxLength = 150;
    public const int EmailMaxLength = 150;
    public const int CargoMaxLength = 100;

    private readonly List<Matricula> _matriculas = [];
    private readonly List<ChamadoSuporte> _chamadosSuporte = [];

    public string Nome { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string? Cargo { get; private set; }

    public bool Ativo { get; private set; }

    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    public IReadOnlyCollection<ChamadoSuporte> ChamadosSuporte => _chamadosSuporte.AsReadOnly();

    private Aluno()
    {
    }

    public Aluno(string nome, string email, string? cargo = null)
    {
        Nome = Guard.Obrigatorio(nome, NomeMaxLength, nameof(Nome));
        Email = ValidarEmail(email);
        Cargo = Guard.Opcional(cargo, CargoMaxLength, nameof(Cargo));
        Ativo = true;
    }

    public void AtualizarDados(string nome, string email, string? cargo)
    {
        Nome = Guard.Obrigatorio(nome, NomeMaxLength, nameof(Nome));
        Email = ValidarEmail(email);
        Cargo = Guard.Opcional(cargo, CargoMaxLength, nameof(Cargo));
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

        if (_matriculas.Exists(item => item.CursoId == matricula.CursoId))
        {
            throw new DomainException("O aluno já está matriculado neste curso.");
        }

        _matriculas.Add(matricula);
    }

    internal void AdicionarChamado(ChamadoSuporte chamado)
    {
        ArgumentNullException.ThrowIfNull(chamado);
        _chamadosSuporte.Add(chamado);
    }

    private static string ValidarEmail(string? email)
    {
        var texto = Guard.Obrigatorio(email, EmailMaxLength, nameof(Email));
        var partes = texto.Split('@');

        if (partes.Length != 2 || partes[0].Length == 0 || !partes[1].Contains('.', StringComparison.Ordinal))
        {
            throw new DomainException("Email inválido.");
        }

        return texto;
    }
}
