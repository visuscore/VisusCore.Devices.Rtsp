namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDeviceUnpublishedEvent : RtspDeviceEvent
{
    public RtspDeviceUnpublishedEvent()
    {
    }

    public RtspDeviceUnpublishedEvent(string deviceId)
        : base(deviceId)
    {
    }
}
