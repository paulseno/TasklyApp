using Microsoft.Extensions.Logging;
using MD_ToDo_List.Services;

namespace MD_ToDo_List;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton(sp => new HttpClient 
        { 
            Timeout = TimeSpan.FromSeconds(30)
        });
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register Pages
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<Pages.SignInPage>();
        builder.Services.AddSingleton<Pages.SignUpPage>();
        builder.Services.AddSingleton<Pages.ProfilePage>();
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        return app;
    }
}