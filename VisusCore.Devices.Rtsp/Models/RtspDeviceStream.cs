using System;
using VisusCore.Devices.Rtsp.Core.Models;

namespace VisusCore.Devices.Rtsp.Models;

public class RtspDeviceStream
{
    public string Id { get; set; }
    public bool Enabled { get; set; }
    public int Port { get; set; }
    public bool PreferTcp { get; set; }
    public string Path { get; set; }
    public EStreamType Type { get; set; }
    public bool AllowAudio { get; set; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not RtspDeviceStream other)
        {
            return false;
        }

        return Id == other.Id
            && Enabled == other.Enabled
            && Port == other.Port
            && PreferTcp == other.PreferTcp
            && Path == other.Path
            && Type == other.Type
            && AllowAudio == other.AllowAudio;
    }

    public override int GetHashCode() =>
        HashCode.Combine(Id, Enabled, Port, PreferTcp, Path, Type, AllowAudio);
}
