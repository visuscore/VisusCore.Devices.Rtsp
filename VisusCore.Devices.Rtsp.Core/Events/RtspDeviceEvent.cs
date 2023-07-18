namespace VisusCore.Devices.Rtsp.Core.Events;

public abstract class RtspDeviceEvent
{
    public string DeviceId { get; set; }

    protected RtspDeviceEvent()
    {
    }

    protected RtspDeviceEvent(string deviceId) =>
        DeviceId = deviceId;
}
