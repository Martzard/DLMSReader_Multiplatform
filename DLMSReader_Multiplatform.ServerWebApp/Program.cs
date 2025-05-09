using DLMSReader_Multiplatform.Shared;
using DLMSReader_Multiplatform.Shared.Components.Data;
using DLMSReader_Multiplatform.Shared.Components.DLMS;
using DLMSReader_Multiplatform.Shared.Components.Services;
using DLMSReader_Multiplatform.Shared.Components.ViewModels;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server
builder.Services.AddRazorPages();        // kvůli _Host.cshtml
builder.Services.AddServerSideBlazor();  // websocket circuit

// moje sluzby
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddTransient<DLMSConnectionManager>();
builder.Services.AddTransient<DeviceConnectionViewModel>();

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
