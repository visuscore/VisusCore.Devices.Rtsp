namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDeviceUpdatedEvent : RtspDeviceEvent
{
    public RtspDeviceUpdatedEvent()
    {
    }

    public RtspDeviceUpdatedEvent(string deviceId)
        : base(deviceId)
    {
    }
}
