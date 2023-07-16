namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDeviceStreamPublishedEvent : RtspDeviceStreamEvent
{
    public RtspDeviceStreamPublishedEvent()
    {
    }

    public RtspDeviceStreamPublishedEvent(string deviceId, string streamId)
        : base(deviceId, streamId)
    {
    }
}
