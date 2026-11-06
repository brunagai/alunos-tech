using System.Security.Claims;

namespace AlunosTech.Api.Auth;

public static class AcessoAluno
{
    public const string PapelGestor = "Gestor";
    public const string PapelColaborador = "Colaborador";

    public static bool PodeOperar(ClaimsPrincipal usuario, Guid alunoId)
    {
        if (usuario.IsInRole(PapelGestor))
        {
            return true;
        }

        var claim = usuario.FindFirst("alunoId")?.Value;
        return Guid.TryParse(claim, out var proprio) && proprio == alunoId;
    }
}
