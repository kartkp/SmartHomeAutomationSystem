namespace smartHome.Api.Services;

using smartHome.Core.Entities;
using smartHome.Core.Interfaces;
using smartHome.Core.Exceptions;
using smartHome.Infrastructure.Data;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _repo;
    private readonly SmartHomeDbContext _context;

    private static Dictionary<int, DateTime> armedDevices = new();

    public DeviceService(IDeviceRepository repo, SmartHomeDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<List<Device>> GetAllDevices()
        => await _repo.GetAll();

    private string NormalizeStatus(string type, string status)
    {
        if (string.IsNullOrWhiteSpace(type))
            return "Off";

        type = type.Trim();
        status = status?.Trim() ?? "";

        if (type == "Light")
        {
            var s = status.ToLower();
            if (s == "on") return "On";
            if (s == "off") return "Off";
            return "Off";
        }

        else if (type == "Lock")
        {
            var s = status.ToLower();
            if (s == "locked") return "Locked";
            if (s == "unlocked") return "Unlocked";
            return "Locked";
        }

        else if (type == "Thermostat")
        {
            if (string.IsNullOrWhiteSpace(status))
                return "24°C";

            var digits = new string(status.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digits))
                return "24°C";

            int temp = int.Parse(digits);

            if (temp < 10) temp = 10;
            if (temp > 40) temp = 40;

            return $"{temp}°C";
        }

        return "Off";
    }

    public async Task AddDevice(Device device)
    {
        if (device == null) return;

        device.Status = NormalizeStatus(device.Type, device.Status);
        await _repo.Add(device);
    }

    public async Task UpdateDevice(int id, string status)
    {
        var device = await _repo.GetById(id);

        if (device == null)
            throw new DeviceNotFoundException("Device not found");

        string normalized = NormalizeStatus(device.Type, status);

        if (IsDeviceArmed(id, out _))
        {
            throw new InvalidCommandException(
                $"Device is ARMED. Cannot change {device.Type} right now!"
            );
        }

        device.Status = normalized;

        await _repo.Update(device);

        _context.CommandHistory.Add(new CommandHistory  //notification data save..
        {
            DeviceId = id,
            Command = device.Status,
            Timestamp = DateTime.Now
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteDevice(int id)
    {
        var device = await _repo.GetById(id);

        if (device == null)
            throw new DeviceNotFoundException("Device not found");

        await _repo.Delete(id);
    }

    public void SetArmedDuration(int deviceId, int seconds)
    {
        if (seconds == 0)
        {
            armedDevices.Remove(deviceId);
            return;
        }

        if (seconds < 0 || seconds > 3600)
        {
            throw new InvalidCommandException(
                "Invalid time. Enter between 1 and 3600 seconds."
            );
        }

        armedDevices[deviceId] = DateTime.Now.AddSeconds(seconds);
    }

    private bool IsDeviceArmed(int deviceId, out int remaining)
    {
        remaining = 0;

        if (!armedDevices.ContainsKey(deviceId))
            return false;

        var endTime = armedDevices[deviceId];

        if (DateTime.Now >= endTime)
        {
            armedDevices.Remove(deviceId);
            return false;
        }

        remaining = (int)(endTime - DateTime.Now).TotalSeconds;
        return true;
    }

    public Dictionary<int, int> GetArmedDevices()
    {
        var result = new Dictionary<int, int>();

        foreach (var item in armedDevices.ToList())
        {
            if (DateTime.Now >= item.Value)
            {
                armedDevices.Remove(item.Key);
            }
            else
            {
                result[item.Key] = (int)(item.Value - DateTime.Now).TotalSeconds;
            }
        }

        return result;
    }
}