using DLMSReader_Multiplatform.Shared.Components.ViewModels;
using Microsoft.Extensions.Logging;
using DLMSReader_Multiplatform.Maui.Services;
using DLMSReader_Multiplatform.Shared.Components.Data;

namespace DLMSReader_Multiplatform.Maui;

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
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<DeviceDataViewModel>();
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "DevicesDB.db3");
        builder.Services.AddSingleton(new DeviceDatabaseService(dbPath));

        System.Diagnostics.Debug.WriteLine("DB PATH: " + FileSystem.AppDataDirectory);


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
