using System.Net.Http.Json;
using DLMSReader_Multiplatform.Shared.Components.Models;

public class DeviceApiService
{
    private readonly HttpClient _http;

    public DeviceApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<DLMSDeviceModel>> GetAllDevicesAsync()
    {
        return await _http.GetFromJsonAsync<List<DLMSDeviceModel>>("api/devices") ?? new();
    }

    public async Task AddDeviceAsync(DLMSDeviceModel device)
    {
        var response = await _http.PostAsJsonAsync("api/devices", device);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteDeviceAsync(int id)
    {
        await _http.DeleteAsync($"api/devices/{id}");
    }
}
