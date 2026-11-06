using AlunosTech.Application.DTOs;
using AlunosTech.Application.Interfaces;
using AlunosTech.Domain.Entities;
using AlunosTech.Domain.Exceptions;

namespace AlunosTech.Application.UseCases;

public class CriarChamadoSuporteUseCase
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IChamadoSuporteRepository _chamadoSuporteRepository;

    public CriarChamadoSuporteUseCase(
        IAlunoRepository alunoRepository,
        IChamadoSuporteRepository chamadoSuporteRepository)
    {
        _alunoRepository = alunoRepository;
        _chamadoSuporteRepository = chamadoSuporteRepository;
    }

    public async Task<Guid> ExecutarAsync(CriarChamadoDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AlunoId == Guid.Empty)
        {
            throw new DomainException("AlunoId é obrigatório.");
        }

        if (!Enum.IsDefined(typeof(PrioridadeChamado), dto.Prioridade))
        {
            throw new DomainException("Prioridade do chamado é inválida. Use 1 (Baixa), 2 (Média) ou 3 (Alta).");
        }

        var aluno = await _alunoRepository.GetByIdAsync(dto.AlunoId, cancellationToken)
            ?? throw new DomainException("Aluno não encontrado.");

        var chamado = new ChamadoSuporte(
            aluno,
            dto.Titulo,
            (PrioridadeChamado)dto.Prioridade,
            dto.Descricao);

        await _chamadoSuporteRepository.AddAsync(chamado, cancellationToken);

        return chamado.Id;
    }
}
