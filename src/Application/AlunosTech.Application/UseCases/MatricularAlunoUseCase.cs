using AlunosTech.Application.DTOs;
using AlunosTech.Application.Interfaces;
using AlunosTech.Domain.Entities;
using AlunosTech.Domain.Exceptions;

namespace AlunosTech.Application.UseCases;

public class MatricularAlunoUseCase
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly IMatriculaRepository _matriculaRepository;

    public MatricularAlunoUseCase(
        IAlunoRepository alunoRepository,
        ICursoRepository cursoRepository,
        IMatriculaRepository matriculaRepository)
    {
        _alunoRepository = alunoRepository;
        _cursoRepository = cursoRepository;
        _matriculaRepository = matriculaRepository;
    }

    public async Task<Guid> ExecutarAsync(MatricularAlunoDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AlunoId == Guid.Empty)
        {
            throw new DomainException("AlunoId é obrigatório.");
        }

        if (dto.CursoId == Guid.Empty)
        {
            throw new DomainException("CursoId é obrigatório.");
        }

        var aluno = await _alunoRepository.GetByIdAsync(dto.AlunoId, cancellationToken)
            ?? throw new DomainException("Aluno não encontrado.");

        var curso = await _cursoRepository.GetByIdAsync(dto.CursoId, cancellationToken)
            ?? throw new DomainException("Curso não encontrado.");

        var matriculas = await _matriculaRepository.GetAllAsync(cancellationToken);

        if (matriculas.Any(matricula => matricula.AlunoId == dto.AlunoId && matricula.CursoId == dto.CursoId))
        {
            throw new DomainException("O aluno já está matriculado neste curso.");
        }

        var matricula = new Matricula(aluno, curso);
        await _matriculaRepository.AddAsync(matricula, cancellationToken);

        return matricula.Id;
    }
}
