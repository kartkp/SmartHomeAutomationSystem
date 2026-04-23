using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartHome.Api.Services;
using smartHome.Core.Entities;
using smartHome.Core.Interfaces;
using smartHome.Api.DTOs;
using smartHome.Infrastructure.Data;
//using smartHome.Api.Services;

namespace smartHome.Api.Controllers; //all linq logics and dto to entity conversion

[ApiController]
[Route("api/devices")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _service;
    private readonly SmartHomeDbContext _context;


    public DeviceController(IDeviceService service, SmartHomeDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var devices = await _context.Devices
            .Include(d => d.Group)
            .ToListAsync();

        var result = devices.Select(d => new DeviceDto
        {
            Id = d.Id,
            Name = d.Name,
            Type = d.Type,
            Status = d.Status,
            GroupId = d.GroupId,
            GroupName = d.Group.GroupName
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddDevice(Device device)
    {
        device.Group = null;

        await _service.AddDevice(device);

        var saved = await _context.Devices
            .Include(d => d.Group)
            .OrderByDescending(d => d.Id)
            .FirstOrDefaultAsync();

        var result = new DeviceDto
        {
            Id = saved.Id,
            Name = saved.Name,
            Type = saved.Type,
            Status = saved.Status,
            GroupId = saved.GroupId,
            GroupName = saved.Group.GroupName
        };

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDeviceDto dto)
    {
        await _service.UpdateDevice(id, dto.Status);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        await _service.DeleteDevice(id);
        return Ok(new { message = "Device deleted successfully.." });
    }

    [HttpPost("set_armed")]
    public IActionResult SetArmed([FromBody] ArmedRequest req)
    {
        _service.SetArmedDuration(req.DeviceId, req.Seconds);
        return Ok();
    }
    [HttpGet("history/recent")]
    public async Task<IActionResult> GetRecentHistory()
    {
        var data = await (
            from ch in _context.CommandHistory
            join d in _context.Devices on ch.DeviceId equals d.Id
            orderby ch.Timestamp descending
            select new
            {
                Name = d.Name,
                Command = ch.Command,
                Timestamp = ch.Timestamp
            }
        )
        .Take(5)
        .ToListAsync();

        return Ok(data);
    }
}