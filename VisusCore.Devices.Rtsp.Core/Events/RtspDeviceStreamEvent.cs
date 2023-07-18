namespace VisusCore.Devices.Rtsp.Core.Events;

public abstract class RtspDeviceStreamEvent : RtspDeviceEvent
{
    public string StreamId { get; set; }

    protected RtspDeviceStreamEvent()
    {
    }

    protected RtspDeviceStreamEvent(string deviceId, string streamId)
        : base(deviceId) =>
        StreamId = streamId;
}
