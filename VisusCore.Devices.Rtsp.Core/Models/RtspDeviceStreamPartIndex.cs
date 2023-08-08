using VisusCore.AidStack.OrchardCore.Parts.Indexing.Models;

namespace VisusCore.Devices.Rtsp.Core.Models;

public class RtspDeviceStreamPartIndex : ContentPartIndex
{
    public int Port { get; set; }
    public bool PreferTcp { get; set; }
    public string Path { get; set; }
    public EStreamType Type { get; set; }
    public bool AllowAudio { get; set; }
}
