using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AlunosTech.Api.Auth;

public class EmissorJwt
{
    private readonly IConfiguration _configuracao;

    public EmissorJwt(IConfiguration configuracao)
    {
        _configuracao = configuracao;
    }

    public string Emitir(string email, string papel, Guid? alunoId)
    {
        var chave = _configuracao["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey não foi configurada.");
        var emissor = _configuracao["Jwt:Issuer"] ?? "AlunosTech";
        var audiencia = _configuracao["Jwt:Audience"] ?? "AlunosTech.Clientes";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, email),
            new(ClaimTypes.Role, papel)
        };

        if (alunoId is Guid id && id != Guid.Empty)
        {
            claims.Add(new Claim("alunoId", id.ToString()));
        }

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: emissor,
            audience: audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
