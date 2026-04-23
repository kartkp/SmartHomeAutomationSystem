using smartHome.App.Services;
using smartHome.Core.DTOs;

namespace smartHome.App.Utilities;

public class Scheduler
{
    private readonly ApiService _api = new();

    public async Task Schedule(DeviceDto device, int seconds)
    {
        if (seconds <= 0 || seconds > 3600)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid time. Enter between 1 and 3600 seconds..");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"\n[Scheduled] {device.Name} will auto-toggle in {seconds} sec");
        await Task.Delay(seconds * 1000);

        string newStatus = "";

        if (device.Type == "Light")
        {
            newStatus = device.Status.ToLower() == "on" ? "Off" : "On";
        }
        else if (device.Type == "Lock")
        {
            newStatus = device.Status.ToLower() == "locked" ? "Unlocked" : "Locked";
        }
        else if (device.Type == "Thermostat")
        {
            newStatus = "25°C";
        }

        await _api.UpdateDevice(device.Id, newStatus);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n{device.Name} set to {newStatus} (auto)");
        Console.ResetColor();
    }
}