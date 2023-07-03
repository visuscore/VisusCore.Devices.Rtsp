using System;
using System.Collections.Generic;
using System.Linq;

namespace VisusCore.Devices.Rtsp.Models;

public class RtspDevice
{
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public string HostName { get; set; }
    public int ConnectionTimeout { get; set; }
    public bool RequireCredentials { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public IEnumerable<RtspDeviceStream> Streams { get; set; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not RtspDevice other)
        {
            return false;
        }

        return Id == other.Id
            && Enabled == other.Enabled
            && HostName == other.HostName
            && ConnectionTimeout == other.ConnectionTimeout
            && RequireCredentials == other.RequireCredentials
            && UserName == other.UserName
            && Password == other.Password
            && Streams?.SequenceEqual(other.Streams ?? Enumerable.Empty<RtspDeviceStream>()) is true;
    }

    public override int GetHashCode() =>
        HashCode.Combine(Id, Enabled, HostName, ConnectionTimeout, RequireCredentials, UserName, Password, Streams);
}
