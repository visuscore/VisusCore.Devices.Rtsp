namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDevicePublishedEvent : RtspDeviceEvent
{
    public RtspDevicePublishedEvent()
    {
    }

    public RtspDevicePublishedEvent(string deviceId)
        : base(deviceId)
    {
    }
}
