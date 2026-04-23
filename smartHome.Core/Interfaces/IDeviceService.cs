using smartHome.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace smartHome.Core.Interfaces;
public interface IDeviceService
{
    Task<List<Device>> GetAllDevices();
    Task UpdateDevice(int id, string status);
    Task AddDevice(Device device);
    Task DeleteDevice(int id);
    void SetArmedDuration(int deviceId, int seconds);
}
