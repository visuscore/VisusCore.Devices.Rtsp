using OrchardCore.ContentManagement;

namespace VisusCore.Devices.Rtsp.Models;

public class RtspDevicePart : ContentPart
{
    public string HostName { get; set; }
    public int ConnectionTimeout { get; set; } = 5;
    public bool RequireCredentials { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
