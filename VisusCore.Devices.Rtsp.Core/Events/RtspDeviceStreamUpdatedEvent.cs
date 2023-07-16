namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDeviceStreamUpdatedEvent : RtspDeviceStreamEvent
{
    public RtspDeviceStreamUpdatedEvent()
    {
    }

    public RtspDeviceStreamUpdatedEvent(string deviceId, string streamId)
        : base(deviceId, streamId)
    {
    }
}
