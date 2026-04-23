using smartHome.Core.DTOs;
using smartHome.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace smartHome.App.Services;

public class ApiService
{
    private readonly HttpClient _client;

    public ApiService()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5199/")
        };
    }

    public async Task<List<DeviceDto>> GetDevices()
    {
        return await _client.GetFromJsonAsync<List<DeviceDto>>("api/devices");
    }

    public async Task UpdateDevice(int id, string status)
    {
        var response = await _client.PutAsJsonAsync($"api/devices/{id}", new { Status = status });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error["message"]);
        }
    }

    public async Task<DeviceDto> AddDevice(DeviceDto device)
{
    var response = await _client.PostAsJsonAsync("api/devices", device);

    var json = await response.Content.ReadAsStringAsync();
    //Console.WriteLine("\nRAW JSON:\n" + json);

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    return JsonSerializer.Deserialize<DeviceDto>(json, options);
}
    public async Task DeleteDevice(int id)
    {
        var response = await _client.DeleteAsync($"api/devices/{id}");

        response.EnsureSuccessStatusCode();
    }
    public async Task SetArmed(int deviceId, int seconds)
    {
        await _client.PostAsJsonAsync("api/devices/set_armed", new
        {
            deviceId,
            seconds
        });
    }
    public class HistoryDto
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public async Task<List<HistoryDto>> GetRecentHistory()
    {
        return await _client.GetFromJsonAsync<List<HistoryDto>>("api/devices/history/recent");
    }

}

