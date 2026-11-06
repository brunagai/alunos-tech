using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AlunosTech.Mobile;

public sealed class ClienteApi
{
    private readonly HttpClient _http;

    public ClienteApi(string? baseUrl = null)
    {
        var endereco = baseUrl
            ?? Environment.GetEnvironmentVariable("ALUNOSTECH_API")
            ?? "http://127.0.0.1:18080";
        _http = new HttpClient { BaseAddress = new Uri(endereco.TrimEnd('/') + "/") };
    }

    public string? Token { get; private set; }

    public async Task<(int Status, string Corpo)> LoginAsync(string email, string senha, CancellationToken cancellationToken = default)
    {
        var resposta = await _http.PostAsJsonAsync("api/auth/login", new { email, senha }, cancellationToken);
        var corpo = await resposta.Content.ReadAsStringAsync(cancellationToken);
        if (resposta.IsSuccessStatusCode)
        {
            var sessao = System.Text.Json.JsonDocument.Parse(corpo);
            Token = sessao.RootElement.GetProperty("token").GetString();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }

        return ((int)resposta.StatusCode, corpo);
    }

    public async Task<(int Status, string Corpo)> MatricularAsync(Guid alunoId, Guid cursoId, CancellationToken cancellationToken = default)
    {
        var resposta = await _http.PostAsJsonAsync("api/matriculas", new { alunoId, cursoId }, cancellationToken);
        return ((int)resposta.StatusCode, await resposta.Content.ReadAsStringAsync(cancellationToken));
    }

    public async Task<(int Status, string Corpo)> AbrirChamadoAsync(Guid alunoId, string titulo, string descricao, int prioridade, CancellationToken cancellationToken = default)
    {
        var resposta = await _http.PostAsJsonAsync(
            "api/chamados",
            new { alunoId, titulo, descricao, prioridade },
            cancellationToken);
        return ((int)resposta.StatusCode, await resposta.Content.ReadAsStringAsync(cancellationToken));
    }

    public static async Task<int> ExecutarChamadaDeVerificacaoAsync()
    {
        try
        {
            return await ExecutarInternoAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FALHA {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static async Task<int> ExecutarInternoAsync()
    {
        var cliente = new ClienteApi();
        var login = await cliente.LoginAsync("colaborador@alunostech.local", "colaborador-dev");
        Console.WriteLine($"LOGIN {login.Status} {login.Corpo}");
        if (login.Status != 200)
        {
            return 1;
        }

        var sessao = System.Text.Json.JsonDocument.Parse(login.Corpo);
        var alunoId = sessao.RootElement.GetProperty("alunoId").GetGuid();
        var matricula = await cliente.MatricularAsync(alunoId, Guid.Parse("22222222-2222-2222-2222-222222222222"));
        Console.WriteLine($"MATRICULA {matricula.Status} {matricula.Corpo}");
        return matricula.Status is 201 or 400 ? 0 : 2;
    }
}
