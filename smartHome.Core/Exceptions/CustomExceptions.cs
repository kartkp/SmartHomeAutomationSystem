using System;
using System.Collections.Generic;
using System.Text;

namespace smartHome.Core.Exceptions;

public class DeviceNotFoundException : Exception
{
    public DeviceNotFoundException(string msg) : base(msg) { }
}

public class InvalidCommandException : Exception
{
    public InvalidCommandException(string msg) : base(msg) { }
}
