using VisusCore.AidStack.OrchardCore.Parts.Indexing.Models;

namespace VisusCore.Devices.Rtsp.Core.Models;

public class RtspDevicePartIndex : ContentPartIndex
{
    public string HostName { get; set; }
    public int ConnectionTimeout { get; set; }
    public bool RequireCredentials { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
