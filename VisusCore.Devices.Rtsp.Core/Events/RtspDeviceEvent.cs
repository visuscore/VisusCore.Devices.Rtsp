namespace VisusCore.Devices.Rtsp.Core.Events;

public abstract class RtspDeviceEvent
{
    protected RtspDeviceEvent()
    {
    }

    protected RtspDeviceEvent(string deviceId) =>
        DeviceId = deviceId;

    public string DeviceId { get; set; }
}
