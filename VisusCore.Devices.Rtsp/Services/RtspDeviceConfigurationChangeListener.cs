using System;
using VisusCore.Devices.Rtsp.Core.Events;
using VisusCore.EventBus.Core.Services;

namespace VisusCore.Devices.Rtsp.Services;

public class RtspDeviceConfigurationChangeListener
{
    private readonly ReactiveEventConsumer<RtspDevicePublishedEvent> _rtspDevicePublished;
    private readonly ReactiveEventConsumer<RtspDeviceRemovedEvent> _rtspDeviceRemoved;
    private readonly ReactiveEventConsumer<RtspDeviceUnpublishedEvent> _rtspDeviceUnpublished;
    private readonly ReactiveEventConsumer<RtspDeviceUpdatedEvent> _rtspDeviceUpdated;

    public IAsyncObservable<RtspDevicePublishedEvent> RtspDevicePublished => _rtspDevicePublished.Events;
    public IAsyncObservable<RtspDeviceRemovedEvent> RtspDeviceRemoved => _rtspDeviceRemoved.Events;
    public IAsyncObservable<RtspDeviceUnpublishedEvent> RtspDeviceUnpublished => _rtspDeviceUnpublished.Events;
    public IAsyncObservable<RtspDeviceUpdatedEvent> RtspDeviceUpdated => _rtspDeviceUpdated.Events;

    public RtspDeviceConfigurationChangeListener(
        ReactiveEventConsumer<RtspDevicePublishedEvent> rtspDevicePublished,
        ReactiveEventConsumer<RtspDeviceRemovedEvent> rtspDeviceRemoved,
        ReactiveEventConsumer<RtspDeviceUnpublishedEvent> rtspDeviceUnpublished,
        ReactiveEventConsumer<RtspDeviceUpdatedEvent> rtspDeviceUpdated)
    {
        _rtspDevicePublished = rtspDevicePublished;
        _rtspDeviceRemoved = rtspDeviceRemoved;
        _rtspDeviceUnpublished = rtspDeviceUnpublished;
        _rtspDeviceUpdated = rtspDeviceUpdated;
    }
}
