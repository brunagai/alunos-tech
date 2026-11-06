using Android.App;
using Android.Content.PM;
using Android.OS;

namespace AlunosTech.Mobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static bool ChamarMatricula { get; set; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var chamada = Intent?.GetStringExtra("chamada");
        var api = Intent?.GetStringExtra("api");
        ChamarMatricula = string.Equals(chamada, "matricula", StringComparison.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(api))
        {
            System.Environment.SetEnvironmentVariable("ALUNOSTECH_API", api);
        }

        base.OnCreate(savedInstanceState);
    }
}
