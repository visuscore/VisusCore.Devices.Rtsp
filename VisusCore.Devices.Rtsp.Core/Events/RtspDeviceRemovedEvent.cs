namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDeviceRemovedEvent : RtspDeviceEvent
{
    public RtspDeviceRemovedEvent()
    {
    }

    public RtspDeviceRemovedEvent(string deviceId)
        : base(deviceId)
    {
    }
}
