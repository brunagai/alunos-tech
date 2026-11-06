using AlunosTech.Api.Auth;
using AlunosTech.Api.Contracts;
using AlunosTech.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlunosTech.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly EmissorJwt _emissor;
    private readonly IAlunoRepository _alunos;
    private readonly IConfiguration _configuracao;

    public AuthController(EmissorJwt emissor, IAlunoRepository alunos, IConfiguration configuracao)
    {
        _emissor = emissor;
        _alunos = alunos;
        _configuracao = configuracao;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
        {
            return BadRequest(new { mensagem = "E-mail e senha são obrigatórios." });
        }

        var emailGestor = _configuracao["Auth:GestorEmail"];
        var senhaGestor = _configuracao["Auth:GestorSenha"];
        if (string.Equals(dto.Email, emailGestor, StringComparison.OrdinalIgnoreCase)
            && dto.Senha == senhaGestor)
        {
            var token = _emissor.Emitir(dto.Email, AcessoAluno.PapelGestor, null);
            return Ok(new { token, papel = AcessoAluno.PapelGestor });
        }

        var senhaColaborador = _configuracao["Auth:ColaboradorSenha"];
        if (dto.Senha != senhaColaborador)
        {
            return Unauthorized(new { mensagem = "Credencial inválida." });
        }

        var alunos = await _alunos.GetAllAsync(cancellationToken);
        var aluno = alunos.FirstOrDefault(item =>
            string.Equals(item.Email, dto.Email, StringComparison.OrdinalIgnoreCase));
        if (aluno is null)
        {
            return Unauthorized(new { mensagem = "Credencial inválida." });
        }

        var tokenColaborador = _emissor.Emitir(aluno.Email, AcessoAluno.PapelColaborador, aluno.Id);
        return Ok(new { token = tokenColaborador, papel = AcessoAluno.PapelColaborador, alunoId = aluno.Id });
    }
}
