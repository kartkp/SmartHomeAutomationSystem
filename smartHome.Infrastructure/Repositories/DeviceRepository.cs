using System;
using System.Collections.Generic;
using System.Text;
namespace smartHome.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using smartHome.Infrastructure.Data;
using smartHome.Core.Entities;
using smartHome.Core.Interfaces;
using smartHome.Infrastructure.Data;

public class DeviceRepository : IDeviceRepository
{
    private readonly SmartHomeDbContext _context;

    public DeviceRepository(SmartHomeDbContext context) //less tight couples  (DI)
    {
        _context = context;
    }

    public async Task<List<Device>> GetAll()
        => await _context.Devices.ToListAsync();

    public async Task<Device> GetById(int id)
        => await _context.Devices.FindAsync(id);

    public async Task Add(Device device)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Device device)
    {
        _context.Devices.Update(device);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var device = await _context.Devices.FindAsync(id);

        if (device != null)
        {
            var history = _context.CommandHistory.Where(c => c.DeviceId == id); //imp for me
            _context.CommandHistory.RemoveRange(history);

            _context.Devices.Remove(device);

            await _context.SaveChangesAsync();
        }
    }
}
