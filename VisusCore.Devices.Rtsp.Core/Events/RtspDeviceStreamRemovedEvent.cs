namespace VisusCore.Devices.Rtsp.Core.Events;

public class RtspDeviceStreamRemovedEvent : RtspDeviceStreamEvent
{
    public RtspDeviceStreamRemovedEvent()
    {
    }

    public RtspDeviceStreamRemovedEvent(string deviceId, string streamId)
        : base(deviceId, streamId)
    {
    }
}
