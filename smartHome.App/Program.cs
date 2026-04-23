using smartHome.App.Events;
using smartHome.App.Services;
using smartHome.App.UI;
using smartHome.App.Utilities;
using smartHome.Core.DTOs;
using smartHome.Core.Exceptions;


namespace smartHome.App;


class Program
{
    static Dictionary<int, DateTime> armedDevices = new();
    static ApiService api = new();
    static Scheduler scheduler = new();
    static Dashboard dashboard = new();
    static Menu menu = new();

    static async Task Main()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                lock (armedDevices)
                {
                    foreach (var key in armedDevices.Keys.ToList())
                    {
                        if ((armedDevices[key] - DateTime.Now).TotalSeconds <= 0)
                            armedDevices.Remove(key);
                    }
                }

                await Task.Delay(1000);
            }
        });
        EventManager.DeviceAdded += (DeviceDto device) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.WriteLine("\n[EVENT] New Device Added:");
            Console.WriteLine($"ID: {device.Id}");
            Console.WriteLine($"Name: {device.Name}");
            Console.WriteLine($"Type: {device.Type}");
            Console.WriteLine($"Status: {device.Status}");
            Console.WriteLine($"Group: {device.GroupName}");

            Console.ResetColor();
        };

        while (true)
        {
            var choice = menu.Show();

            switch (choice)
            {
                case 1:
                    await ShowDevices();
                    break;

                case 2:
                    await ControlDevice();
                    break;

                case 3:
                    await DiscoverDevice();
                    break;

                case 4:
                    await ScheduleDevice();
                    break;

                case 5:
                    await RemoveDevice();
                    break;

                case 6:
                    return;
            }
        }
    }
    static async Task ShowDevices()
    {
        Console.Write("[Scanning");
        for (int i = 0; i < 5; i++)
        {
            Console.Write(".");
            await Task.Delay(200);
        }
        Console.WriteLine("]");
        try
        {
            var devices = await api.GetDevices();

            if (devices == null || !devices.Any())
            {
                Console.WriteLine("no devices available!!");
                return;
            }

            dashboard.Show(devices, armedDevices);
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("API not reachable!!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Pause();
    }

    static async Task ControlDevice()
    {
        Console.Write("Fetching");
        for (int i = 0; i < 3; i++)
        {
            Console.Write(".");
            await Task.Delay(200);
        }
        Console.Clear();
        while (true)
        {

            Console.WriteLine();
            try
            {
                var devices = await api.GetDevices();

                if (devices == null || !devices.Any())
                {
                    ShowError("No devices found");
                    Pause();
                    return;
                }

                int selected = 0;

                while (true)
                {
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"Devices: {devices.Count} | Time: {DateTime.Now:T}");
                    Console.ResetColor();
                    Console.WriteLine();

                    Console.WriteLine("Use arrow keys to select, Enter to confirm (ESC to go back)\n");

                    for (int i = 0; i < devices.Count; i++)
                    {
                        if (i == selected)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        PrintDeviceWithArmed(devices[i], i + 1);
                        Console.ResetColor();
                    }

                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.UpArrow && selected > 0) selected--;
                    else if (key == ConsoleKey.DownArrow && selected < devices.Count - 1) selected++;
                    else if (key == ConsoleKey.Enter) break;
                    else if (key == ConsoleKey.Escape) return;
                }

                var device = devices[selected];

                Console.WriteLine($"\nSelected: {device.Name} ({device.Type})");

                string cmd = "";

                if (device.Type == "Light")
                {
                    string[] options = { "On", "Off" };
                    int opt = device.Status.ToLower() == "on" ? 0 : 1;


                    while (true)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Device: {device.Name} (Light)");

                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"Current Status: {device.Status}\n");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Use arrow keys and Enter (ESC to go back)\n");

                        for (int i = 0; i < options.Length; i++)
                        {
                            bool isSelected = i == opt;
                            bool isSuggested = options[i].ToLower() != device.Status.ToLower();

                            if (isSelected)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                if (options[i] == "On")
                                    Console.ForegroundColor = ConsoleColor.Green;
                                else
                                    Console.ForegroundColor = ConsoleColor.Red;
                            }

                            Console.Write(options[i]);

                            if (isSuggested)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("  (Suggested)");
                            }

                            Console.ResetColor();
                            Console.WriteLine();
                        }

                        var key = Console.ReadKey(true).Key;

                        if (key == ConsoleKey.UpArrow && opt > 0) opt--;
                        else if (key == ConsoleKey.DownArrow && opt < options.Length - 1) opt++;
                        else if (key == ConsoleKey.Enter)
                        {
                            cmd = options[opt];
                            break;
                        }
                        else if (key == ConsoleKey.Escape) return;
                    }
                }

                else if (device.Type == "Lock")
                {
                    string[] options = { "Locked", "Unlocked" };
                    int opt = device.Status.ToLower() == "locked" ? 0 : 1;


                    while (true)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Device: {device.Name} (Lock)");

                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"Current Status: {device.Status}\n");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Use arrow keys and Enter (ESC to go back)\n");

                        for (int i = 0; i < options.Length; i++)
                        {
                            bool isSelected = i == opt;
                            bool isSuggested = options[i].ToLower() != device.Status.ToLower();

                            if (isSelected)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                if (options[i] == "Locked")
                                    Console.ForegroundColor = ConsoleColor.Red;
                                else
                                    Console.ForegroundColor = ConsoleColor.Green;
                            }

                            Console.Write(options[i]);

                            if (isSuggested)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("  (Suggested)");
                            }

                            Console.ResetColor();
                            Console.WriteLine();
                        }

                        var key = Console.ReadKey(true).Key;

                        if (key == ConsoleKey.UpArrow && opt > 0) opt--;
                        else if (key == ConsoleKey.DownArrow && opt < options.Length - 1) opt++;
                        else if (key == ConsoleKey.Enter)
                        {
                            cmd = options[opt];
                            break;
                        }
                        else if (key == ConsoleKey.Escape) return;
                    }
                }

                else if (device.Type == "Thermostat")
                {
                    while (true)
                    {
                        Console.WriteLine($"Current Temperature: {device.Status}");
                        Console.Write("Enter Temperature (e.g. 24) (0 to go back): ");
                        cmd = Console.ReadLine();

                        if (cmd == "0") return;

                        if (int.TryParse(cmd, out int temp))
                        {
                            if (temp >= 10 && temp <= 35)
                            {
                                cmd = $"{temp}°C";
                                break;
                            }
                        }

                        ShowError("invalid temperature (10–35 only)");
                    }
                }

                await api.UpdateDevice(device.Id, cmd);

                Console.Write("\nEnter armed duration (seconds) (0 to skip): ");
                var input2 = Console.ReadLine();

                int seconds = 0;

                while (true)
                {
                    if (input2 == "0" || input2?.ToLower() == "n" || input2?.ToLower() == "na" || input2?.ToLower() == "n/a" || input2?.ToLower() == "no")
                        break;

                    if (!int.TryParse(input2, out seconds) || seconds <= 0 || seconds > 3600)
                    {
                        ShowError("Enter valid time (1–3600 sec)");
                        Console.Write("enter again: ");
                        input2 = Console.ReadLine();
                        continue;
                    }

                    break;
                }

                if (int.TryParse(input2, out seconds) && seconds > 0)
                {
                    await api.SetArmed(device.Id, seconds);

                    lock (armedDevices)
                    {
                        armedDevices[device.Id] = DateTime.Now.AddSeconds(seconds);
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"System armed for {seconds} seconds");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" {device.Name} is set to: {cmd}");
                Console.ResetColor();

                Pause();
                return;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }

    static async Task DiscoverDevice()
    {
        try
        {
            Console.Write("[Scanning for new devices");
            for (int i = 0; i < 5; i++)
            {
                Console.Write(".");
                await Task.Delay(200);
            }
            Console.WriteLine("]");


            var rnd = new Random();

            string[] types = { "Light", "Lock", "Thermostat" };
            string type = types[rnd.Next(types.Length)];

            string status = type switch
            {
                "Light" => rnd.Next(2) == 0 ? "On" : "Off",

                "Lock" => rnd.Next(2) == 0 ? "Locked" : "Unlocked",

                "Thermostat" => $"{rnd.Next(16, 35)}°C",

                _ => "Off"
            };

            string[] rooms = { "Living Room", "Bedroom", "Kitchen", "Hall" };
            string room = rooms[rnd.Next(rooms.Length)];

            int groupId = room switch
            {
                "Living Room" => 1,
                "Bedroom" => 2,
                "Kitchen" => 3,
                "Hall" => 4,
                _ => 1
            };

            var device = new DeviceDto
            {
                Name = $"{type}_{rnd.Next(100, 999)}_{DateTime.Now.Millisecond}",
                Type = type,
                Status = status,
                GroupId = groupId
            };

            var savedDevice = await api.AddDevice(device);

            EventManager.Raise(savedDevice);
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("api not reachable!!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Pause();
    }

    static async Task ScheduleDevice()
    {
        try
        {
            Console.Write("Fetching");
            for (int i = 0; i < 3; i++)
            {
                Console.Write(".");
                await Task.Delay(200);
            }
            Console.Clear();
            List<DeviceDto> devices = await api.GetDevices();

            if (devices == null || !devices.Any())
            {
                Console.WriteLine("No devices found!");
                return;
            }

            int selected = 0;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Devices: {devices.Count} | Time: {DateTime.Now:T}");
                Console.ResetColor();
                Console.WriteLine();

                Console.WriteLine("Use arrow keys to select device, Enter to confirm (ESC to go back)\n");

                for (int i = 0; i < devices.Count; i++)
                {
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    PrintDeviceWithArmed(devices[i], i + 1);
                    Console.ResetColor();
                }

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && selected > 0) selected--;
                else if (key == ConsoleKey.DownArrow && selected < devices.Count - 1) selected++;
                else if (key == ConsoleKey.Enter) break;
                else if (key == ConsoleKey.Escape) return;
            }

            var device = devices[selected];

            int seconds;

            while (true)
            {
                Console.WriteLine();
                Console.Write("Enter delay (seconds) (0 to go back): ");
                var input = Console.ReadLine();

                if (input == "0") return;

                if (!int.TryParse(input, out seconds) || seconds <= 0 || seconds > 3600)
                {
                    ShowError("Invalid time");
                    continue;
                }

                break;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nDevice '{device.Name}' will be scheduled after {seconds} seconds.");
            Console.ResetColor();
            _ = Task.Run(async () =>
            {
                try
                {
                    await scheduler.Schedule(device, seconds);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nSchedule error: {ex.Message}");
                    Console.ResetColor();
                }
            });
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("API not reachable!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Pause();
    }
    static async Task RemoveDevice()
    {
        try
        {
            Console.Write("Fetching");
            for (int i = 0; i < 3; i++)
            {
                Console.Write(".");
                await Task.Delay(200);
            }
            Console.Clear();
            List<DeviceDto> devices = await api.GetDevices();

            if (devices == null || !devices.Any())
            {
                Console.WriteLine("No devices available!");
                return;
            }

            int selected = 0;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Devices: {devices.Count} | Time: {DateTime.Now:T}");
                Console.ResetColor();
                Console.WriteLine();

                Console.WriteLine("Use arrow keys to select device to remove, Enter to confirm (ESC to go back)\n");

                for (int i = 0; i < devices.Count; i++)
                {
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine($"{i + 1}. {devices[i].Name} ({devices[i].Status})");
                    Console.ResetColor();
                }

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && selected > 0) selected--;
                else if (key == ConsoleKey.DownArrow && selected < devices.Count - 1) selected++;
                else if (key == ConsoleKey.Enter) break;
                else if (key == ConsoleKey.Escape) return;
            }

            var device = devices[selected];

            if (armedDevices.ContainsKey(device.Id))
            {
                int remaining = (int)(armedDevices[device.Id] - DateTime.Now).TotalSeconds;

                if (remaining > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nDevice '{device.Name}' is currently ARMED ({remaining}s left).");
                    Console.WriteLine("You cannot remove an armed device.");
                    Console.ResetColor();

                    Pause();
                    return;
                }
            }

            Console.WriteLine($"\nAre you sure you want to delete '{device.Name}'? (y/n): ");
            var confirm = Console.ReadLine()?.ToLower();

            if (confirm != "y" && confirm != "yes")
            {
                Console.WriteLine("Cancelled..");
                return;
            }

            await api.DeleteDevice(device.Id);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Device removed successfully!");
            Console.ResetColor();
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("API not reachable!!");
        }
        catch (DeviceNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Pause();
    }
    static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nPress any key to continue...");
        Console.ResetColor();
        Console.ReadKey();
    }

    static void ShowError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{msg}");
        Console.ResetColor();

        Thread.Sleep(1000);
    }

    static void PrintDeviceWithArmed(DeviceDto d, int index)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{index}. ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{d.Name} ");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"({d.Type}) ");

        if (d.Status.ToLower().Contains("on") || d.Status.ToLower().Contains("unlocked"))
            Console.ForegroundColor = ConsoleColor.Green;
        else if (d.Status.ToLower().Contains("off") || d.Status.ToLower().Contains("locked"))
            Console.ForegroundColor = ConsoleColor.Red;
        else
            Console.ForegroundColor = ConsoleColor.Yellow;

        Console.Write($"{d.Status}");

        if (armedDevices.ContainsKey(d.Id))
        {
            int remaining = (int)(armedDevices[d.Id] - DateTime.Now).TotalSeconds;
            if (remaining > 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($" [{remaining}s]");
            }
        }

        Console.ResetColor();
        Console.WriteLine();
    }
}