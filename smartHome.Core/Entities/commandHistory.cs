using System;
using System.Collections.Generic;
using System.Text;
namespace smartHome.Core.Entities;
public class CommandHistory
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string Command { get; set; }
    public DateTime Timestamp { get; set; }
}
