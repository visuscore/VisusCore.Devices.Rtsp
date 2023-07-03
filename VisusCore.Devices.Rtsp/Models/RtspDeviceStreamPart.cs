using OrchardCore.ContentManagement;
using VisusCore.Devices.Rtsp.Core.Models;

namespace VisusCore.Devices.Rtsp.Models;

public class RtspDeviceStreamPart : ContentPart
{
    public bool Enabled { get; set; } = true;
    public int Port { get; set; } = 554;
    public bool PreferTcp { get; set; }
    public string Path { get; set; }
    public EStreamType Type { get; set; } = EStreamType.Primary;
    public bool AllowAudio { get; set; }
}
