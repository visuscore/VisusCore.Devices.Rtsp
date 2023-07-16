namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDeviceStreamUnpublishedEvent : RtspDeviceStreamEvent
{
    public RtspDeviceStreamUnpublishedEvent()
    {
    }

    public RtspDeviceStreamUnpublishedEvent(string deviceId, string streamId)
        : base(deviceId, streamId)
    {
    }
}
