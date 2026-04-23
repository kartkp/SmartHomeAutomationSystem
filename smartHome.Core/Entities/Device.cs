using System;
using System.Collections.Generic;
using System.Text;

namespace smartHome.Core.Entities;
public class Device
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public int GroupId { get; set; }
    public DeviceGroup? Group { get; set; }
}
