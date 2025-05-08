using DLMSReader_Multiplatform.Shared;
using DLMSReader_Multiplatform.Shared.Components.Data;
using DLMSReader_Multiplatform.Shared.Components.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server (klasicky)
builder.Services.AddRazorPages();        // kvůli _Host.cshtml
builder.Services.AddServerSideBlazor();  // websocket circuit

// tvoje služby
var dbPath = Path.Combine(AppContext.BaseDirectory, "devices.db3");
builder.Services.AddSingleton(new DeviceDatabaseService(dbPath));
builder.Services.AddSingleton<DeviceDataViewModel>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();                 // /_blazor websocket
app.MapFallbackToPage("/_Host");    // host-stránka s komponentou

app.Run();
