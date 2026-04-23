namespace smartHome.Core.Entities;

public class DeviceGroup
{
    public int Id { get; set; }
    public string GroupName { get; set; }

    public List<Device> Devices { get; set; }
}