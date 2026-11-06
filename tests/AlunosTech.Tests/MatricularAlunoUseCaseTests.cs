using AlunosTech.Application.DTOs;
using AlunosTech.Application.Interfaces;
using AlunosTech.Application.UseCases;
using AlunosTech.Domain.Entities;
using AlunosTech.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace AlunosTech.Tests;

public class MatricularAlunoUseCaseTests
{
    private readonly Mock<IAlunoRepository> _alunoRepository = new();
    private readonly Mock<ICursoRepository> _cursoRepository = new();
    private readonly Mock<IMatriculaRepository> _matriculaRepository = new();

    [Fact]
    public async Task DeveMatricularAlunoExistenteEmCursoExistenteSemDuplicidade()
    {
        var aluno = new Aluno("Ana Souza", "ana.souza@alunostech.com", "Analista");
        var curso = new Curso("Arquitetura .NET", 40, "Clean Architecture");
        Matricula? matriculaSalva = null;

        _alunoRepository
            .Setup(repositorio => repositorio.GetByIdAsync(aluno.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);
        _cursoRepository
            .Setup(repositorio => repositorio.GetByIdAsync(curso.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);
        _matriculaRepository
            .Setup(repositorio => repositorio.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Matricula>());
        _matriculaRepository
            .Setup(repositorio => repositorio.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()))
            .Callback<Matricula, CancellationToken>((matricula, _) => matriculaSalva = matricula)
            .Returns(Task.CompletedTask);

        var id = await CriarCasoDeUso().ExecutarAsync(new MatricularAlunoDto
        {
            AlunoId = aluno.Id,
            CursoId = curso.Id
        });

        matriculaSalva.Should().NotBeNull();
        id.Should().Be(matriculaSalva!.Id);
        matriculaSalva.AlunoId.Should().Be(aluno.Id);
        matriculaSalva.CursoId.Should().Be(curso.Id);
        matriculaSalva.Status.Should().Be(StatusMatricula.Ativa);
        _matriculaRepository.Verify(
            repositorio => repositorio.AddAsync(matriculaSalva, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeveLancarExcecaoDeDominioQuandoAlunoNaoForEncontrado()
    {
        var alunoId = Guid.NewGuid();
        var cursoId = Guid.NewGuid();

        _alunoRepository
            .Setup(repositorio => repositorio.GetByIdAsync(alunoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Aluno?)null);

        var acao = () => CriarCasoDeUso().ExecutarAsync(new MatricularAlunoDto
        {
            AlunoId = alunoId,
            CursoId = cursoId
        });

        await acao.Should().ThrowAsync<DomainException>()
            .WithMessage("Aluno não encontrado.");
        _matriculaRepository.Verify(
            repositorio => repositorio.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeveLancarExcecaoDeDominioQuandoCursoNaoForEncontrado()
    {
        var aluno = new Aluno("Bruno Lima", "bruno.lima@alunostech.com");
        var cursoId = Guid.NewGuid();

        _alunoRepository
            .Setup(repositorio => repositorio.GetByIdAsync(aluno.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);
        _cursoRepository
            .Setup(repositorio => repositorio.GetByIdAsync(cursoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        var acao = () => CriarCasoDeUso().ExecutarAsync(new MatricularAlunoDto
        {
            AlunoId = aluno.Id,
            CursoId = cursoId
        });

        await acao.Should().ThrowAsync<DomainException>()
            .WithMessage("Curso não encontrado.");
        _matriculaRepository.Verify(
            repositorio => repositorio.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoMatriculaJaExistir()
    {
        var aluno = new Aluno("Carla Dias", "carla.dias@alunostech.com");
        var curso = new Curso("PostgreSQL", 24);
        var matriculaExistente = new Matricula(aluno, curso);

        _alunoRepository
            .Setup(repositorio => repositorio.GetByIdAsync(aluno.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aluno);
        _cursoRepository
            .Setup(repositorio => repositorio.GetByIdAsync(curso.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);
        _matriculaRepository
            .Setup(repositorio => repositorio.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { matriculaExistente });

        var acao = () => CriarCasoDeUso().ExecutarAsync(new MatricularAlunoDto
        {
            AlunoId = aluno.Id,
            CursoId = curso.Id
        });

        await acao.Should().ThrowAsync<DomainException>()
            .WithMessage("O aluno já está matriculado neste curso.");
        _matriculaRepository.Verify(
            repositorio => repositorio.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private MatricularAlunoUseCase CriarCasoDeUso()
    {
        return new MatricularAlunoUseCase(
            _alunoRepository.Object,
            _cursoRepository.Object,
            _matriculaRepository.Object);
    }
}
