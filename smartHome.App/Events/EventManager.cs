using smartHome.Core.DTOs;

namespace smartHome.App.Events;

public static class EventManager
{
    public static event Action<DeviceDto> DeviceAdded;

    public static void Raise(DeviceDto device)
    {
        DeviceAdded?.Invoke(device);
    }
}