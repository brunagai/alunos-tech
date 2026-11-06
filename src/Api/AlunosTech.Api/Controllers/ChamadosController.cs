using AlunosTech.Api.Auth;
using AlunosTech.Application.DTOs;
using AlunosTech.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlunosTech.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChamadosController : ControllerBase
{
    private readonly CriarChamadoSuporteUseCase _criarChamadoSuporteUseCase;

    public ChamadosController(CriarChamadoSuporteUseCase criarChamadoSuporteUseCase)
    {
        _criarChamadoSuporteUseCase = criarChamadoSuporteUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarChamadoDto dto,
        CancellationToken cancellationToken)
    {
        if (!AcessoAluno.PodeOperar(User, dto.AlunoId))
        {
            return Forbid();
        }

        var id = await _criarChamadoSuporteUseCase.ExecutarAsync(dto, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new { id });
    }
}
