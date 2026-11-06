using AlunosTech.Api.Auth;
using AlunosTech.Application.DTOs;
using AlunosTech.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlunosTech.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MatriculasController : ControllerBase
{
    private readonly MatricularAlunoUseCase _matricularAlunoUseCase;

    public MatriculasController(MatricularAlunoUseCase matricularAlunoUseCase)
    {
        _matricularAlunoUseCase = matricularAlunoUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Matricular(
        [FromBody] MatricularAlunoDto dto,
        CancellationToken cancellationToken)
    {
        if (!AcessoAluno.PodeOperar(User, dto.AlunoId))
        {
            return Forbid();
        }

        var id = await _matricularAlunoUseCase.ExecutarAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(Matricular), new { id }, new { id });
    }
}
