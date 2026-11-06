namespace AlunosTech.Application.DTOs;

public class CriarChamadoDto
{
    public Guid AlunoId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public int Prioridade { get; set; }
}
