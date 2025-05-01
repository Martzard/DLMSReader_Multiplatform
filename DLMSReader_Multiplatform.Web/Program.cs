using DLMSReader_Multiplatform.Shared.Components.Data;
using DLMSReader_Multiplatform.Shared.Components.Models;

var builder = WebApplication.CreateBuilder(args);

// === Register services ===
var dbPath = Path.Combine("Data", "devices.db");
Directory.CreateDirectory("Data");

builder.Services.AddSingleton(new DeviceDatabaseService(dbPath));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// === Devices endpoints ===

app.MapGet("/api/devices", (DeviceDatabaseService db) =>
{
    return Results.Ok(db.GetAllDevices());
});

app.MapPost("/api/devices", (DeviceDatabaseService db, DLMSDeviceModel device) =>
{
    db.SaveDevice(device);
    return Results.Ok(device);
});

app.MapDelete("/api/devices/{id:int}", (DeviceDatabaseService db, int id) =>
{
    db.DeleteDevice(id);
    return Results.NoContent();
});

// === Example: WeatherForecast ===

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
