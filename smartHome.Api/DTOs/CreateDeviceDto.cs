namespace smartHome.Api.DTOs
{
    public class CreateDeviceDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int GroupId { get; set; }
    }
}
