// Program.cs ─ DLMSReader_Multiplatform.Web
using DLMSReader_Multiplatform.Shared;                       // App.razor + Pages v RCL
using DLMSReader_Multiplatform.Shared.Components.ViewModels; // tvoje ViewModely
using DLMSReader_Multiplatform.Shared.Components.Data;       // SQLite DAL

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────
// 1) Služby
// ──────────────────────────────────────────────────
builder.Services.AddRazorPages();        // _Host.cshtml je Razor Page
builder.Services.AddServerSideBlazor();  // Blazor Server (SignalR circuit)

// antiforgery tokeny (vyžaduje je Razor Pages & Blazor od .NET 8)
builder.Services.AddAntiforgery();

// tvoje perzistentní služby
var dbPath = Path.Combine(AppContext.BaseDirectory, "devices.db3");
builder.Services.AddSingleton(new DeviceDatabaseService(dbPath));
builder.Services.AddSingleton<DeviceDataViewModel>();

var app = builder.Build();

// ──────────────────────────────────────────────────
// 2) Middleware pipeline
// ──────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();   // detailní chyby
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();  // volitelné, ale běžné
app.UseStaticFiles();       // servíruje _framework/blazor.server.js

app.UseRouting();           // povinné pro endpoint routing
app.UseAntiforgery();       // MUSÍ být za UseRouting a před mapováním endpointů

// ──────────────────────────────────────────────────
// 3) Mapování endpointů
// ──────────────────────────────────────────────────
app.MapBlazorHub();                 // SignalR hub /_blazor
app.MapFallbackToPage("/_Host");    // Razor Page, která hostí <component>

// hotovo
app.Run();
