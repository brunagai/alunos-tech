namespace AlunosTech.Mobile;

public partial class MainPage : ContentPage
{
    private readonly ClienteApi _cliente = new();
    private Guid _alunoId;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
#if ANDROID
        if (!MainActivity.ChamarMatricula)
        {
            return;
        }

        MainActivity.ChamarMatricula = false;
        var codigo = await ClienteApi.ExecutarChamadaDeVerificacaoAsync();
        Saida.Text = $"Verificação concluída com código {codigo}.";
#endif
    }

    private async void OnEntrar(object? sender, EventArgs e)
    {
        var resultado = await _cliente.LoginAsync(Email.Text ?? "", Senha.Text ?? "");
        if (resultado.Status == 200)
        {
            using var json = System.Text.Json.JsonDocument.Parse(resultado.Corpo);
            _alunoId = json.RootElement.GetProperty("alunoId").GetGuid();
        }

        Saida.Text = $"{resultado.Status} {resultado.Corpo}";
    }

    private async void OnMatricular(object? sender, EventArgs e)
    {
        if (!Guid.TryParse(CursoId.Text, out var cursoId))
        {
            Saida.Text = "CursoId inválido.";
            return;
        }

        var resultado = await _cliente.MatricularAsync(_alunoId, cursoId);
        Saida.Text = $"{resultado.Status} {resultado.Corpo}";
    }

    private async void OnChamado(object? sender, EventArgs e)
    {
        var resultado = await _cliente.AbrirChamadoAsync(_alunoId, Titulo.Text ?? "", "Aberto pelo aplicativo MAUI.", 2);
        Saida.Text = $"{resultado.Status} {resultado.Corpo}";
    }
}
