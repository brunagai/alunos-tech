using Microsoft.Extensions.Logging;

namespace AlunosTech.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		if (string.Equals(Environment.GetEnvironmentVariable("ALUNOSTECH_CHAMADA"), "matricula", StringComparison.OrdinalIgnoreCase))
		{
			var codigo = ClienteApi.ExecutarChamadaDeVerificacaoAsync().GetAwaiter().GetResult();
			Environment.Exit(codigo);
		}

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
