namespace smartHome.Api.DTOs;
public class DeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string Type { get; set; }
    public int DeviceId { get; set; }
    public int Seconds { get; set; }
}