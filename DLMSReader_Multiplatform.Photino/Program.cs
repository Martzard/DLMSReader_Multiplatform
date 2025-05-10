using DLMSReader_Multiplatform.Photino.Components;
using DLMSReader_Multiplatform.Shared.Components.Data;
using DLMSReader_Multiplatform.Shared.Components.DLMS;
using DLMSReader_Multiplatform.Shared.Components.Services;
using DLMSReader_Multiplatform.Shared.Components.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"DLMSReader_Multiplatform");

        Directory.CreateDirectory(configDir);

        string dbPath = Path.Combine(configDir, "devicesDB.db");

        var dbService = new DeviceDatabaseService(dbPath);
        var viewModel = new DeviceDataViewModel(dbService);

        appBuilder.Services.AddSingleton<ILogService, LogService>();
        appBuilder.Services.AddTransient<DLMSConnectionManager>();
        appBuilder.Services.AddTransient<DeviceConnectionViewModel>();
        appBuilder.Services.AddLogging();
        appBuilder.Services.AddSingleton(dbService);
        appBuilder.Services.AddSingleton(viewModel);
        appBuilder.RootComponents.Add<App>("app");
            

        Console.WriteLine("SQLite path: " + dbPath);


        var app = appBuilder.Build();


        app.MainWindow.SetTitle("DLMSReader Multiplatform");
        

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal Exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }
}