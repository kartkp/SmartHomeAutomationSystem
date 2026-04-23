using System;
using System.Collections.Generic;
using System.Linq;
using smartHome.Core.DTOs;

namespace smartHome.App.UI;

public class Dashboard
{
    public void Show(List<DeviceDto> devices, Dictionary<int, DateTime> armedDevices)
    {
        Console.Clear();

        var grouped = devices.GroupBy(d => d.GroupName);

        foreach (var group in grouped)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n===== {group.Key} =====");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{"Name",-20} {"Type",-12} {"Status",-12} {"Armed",-10}");
            Console.WriteLine(new string('-', 60));
            Console.ResetColor();

            foreach (var d in group)
            {
                Console.ForegroundColor = GetColor(d.Status);

                string armedText = "";

                if (armedDevices.ContainsKey(d.Id))
                {
                    int remaining = (int)(armedDevices[d.Id] - DateTime.Now).TotalSeconds;

                    if (remaining > 0)
                        armedText = $"{remaining}s";
                }

                Console.WriteLine($"{d.Name,-20} {d.Type,-12} {d.Status,-12} {armedText,-10}");
            }

            Console.ResetColor();
        }
    }

    private ConsoleColor GetColor(string status)
    {
        status = status.ToLower();

        if (status.Contains("on") || status.Contains("unlocked"))
            return ConsoleColor.Green;

        if (status.Contains("off") || status.Contains("locked"))
            return ConsoleColor.Red;

        if (status.Contains("°c"))
            return ConsoleColor.Blue;

        return ConsoleColor.White;
    }
}