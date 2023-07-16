namespace VisusCore.Devices.Rtsp.Core.Events;

public abstract class RtspDeviceStreamEvent : RtspDeviceEvent
{
    protected RtspDeviceStreamEvent()
    {
    }

    protected RtspDeviceStreamEvent(string deviceId, string streamId)
        : base(deviceId) =>
        StreamId = streamId;

    public string StreamId { get; set; }
}
