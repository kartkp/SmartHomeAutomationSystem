using smartHome.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace smartHome.Core.Interfaces;
public interface IDeviceRepository
{
    Task<List<Device>> GetAll();
    Task<Device> GetById(int id);
    Task Add(Device device);
    Task Update(Device device);

    Task Delete(int id);
}
