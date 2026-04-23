using System;

namespace smartHome.App.UI;

using smartHome.App.Services;
using static smartHome.App.Services.ApiService;

public class Menu
{
    public int Show()
    {
        int selected = 0;

        string[] options =
        {
            "View Devices",
            "Control Device",
            "Discover Device",
            "Schedule Device",
            "Remove Device",
            "Exit"
        };

        ConsoleColor[] colors =
        {
            ConsoleColor.White,
            ConsoleColor.White,
            ConsoleColor.White,
            ConsoleColor.White,
            ConsoleColor.Red,
            ConsoleColor.White
        };

        var api = new ApiService();
        List<HistoryDto> logs = null;

        try
        {
            Console.Clear();
            logs = api.GetRecentHistory().Result;
        }
        catch
        {
            logs = null;
        }

        Console.CursorVisible = false;

        while (true)
        {

            Console.SetCursorPosition(0, 0);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n========================================");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("      SMART HOME AUTOMATION SYSTEM");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("========================================\n");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{DateTime.Now}\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = colors[i];
                }

                Console.WriteLine($"[{i + 1}] {options[i]}        ");
                Console.ResetColor();
            }

            Console.WriteLine("\nUse arrow keys to navigate, Enter to select");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n===== Recent changes =====");

            if (logs == null || logs.Count == 0)
            {
                Console.WriteLine("No recent activity       \n");
            }
            else
            {
                foreach (var log in logs)
                {
                    Console.WriteLine(
                        $"{log.Name} | {log.Command} | {log.Timestamp:HH:mm:ss}       "
                    );
                }
                Console.WriteLine();
            }

            Console.ResetColor();

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && selected > 0) selected--;
            else if (key == ConsoleKey.DownArrow && selected < options.Length - 1) selected++;
            else if (key == ConsoleKey.Enter)
            {
                try
                {
                    logs = api.GetRecentHistory().Result;
                }
                catch { }

                return selected + 1;
            }
        }
    }
}