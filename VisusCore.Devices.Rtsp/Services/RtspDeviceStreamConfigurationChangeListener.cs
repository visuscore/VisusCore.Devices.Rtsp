using System;
using VisusCore.Devices.Rtsp.Core.Events;
using VisusCore.EventBus.Core.Services;

namespace VisusCore.Devices.Rtsp.Services;

public class RtspDeviceStreamConfigurationChangeListener
{
    private readonly ReactiveEventConsumer<RtspDeviceStreamPublishedEvent> _rtspDeviceStreamPublished;
    private readonly ReactiveEventConsumer<RtspDeviceStreamRemovedEvent> _rtspDeviceStreamRemoved;
    private readonly ReactiveEventConsumer<RtspDeviceStreamUnpublishedEvent> _rtspDeviceStreamUnpublished;
    private readonly ReactiveEventConsumer<RtspDeviceStreamUpdatedEvent> _rtspDeviceStreamUpdated;

    public IAsyncObservable<RtspDeviceStreamPublishedEvent> RtspDeviceStreamPublishet => _rtspDeviceStreamPublished.Events;
    public IAsyncObservable<RtspDeviceStreamRemovedEvent> RtspDeviceStreamRemoved => _rtspDeviceStreamRemoved.Events;
    public IAsyncObservable<RtspDeviceStreamUnpublishedEvent> RtspDeviceStreamUnpublished => _rtspDeviceStreamUnpublished.Events;
    public IAsyncObservable<RtspDeviceStreamUpdatedEvent> RtspDeviceStreamUpdated => _rtspDeviceStreamUpdated.Events;

    public RtspDeviceStreamConfigurationChangeListener(
        ReactiveEventConsumer<RtspDeviceStreamPublishedEvent> rtspDeviceStreamPublished,
        ReactiveEventConsumer<RtspDeviceStreamRemovedEvent> rtspDeviceStreamRemoved,
        ReactiveEventConsumer<RtspDeviceStreamUnpublishedEvent> rtspDeviceStreamUnpublished,
        ReactiveEventConsumer<RtspDeviceStreamUpdatedEvent> rtspDeviceStreamUpdated)
    {
        _rtspDeviceStreamPublished = rtspDeviceStreamPublished;
        _rtspDeviceStreamRemoved = rtspDeviceStreamRemoved;
        _rtspDeviceStreamUnpublished = rtspDeviceStreamUnpublished;
        _rtspDeviceStreamUpdated = rtspDeviceStreamUpdated;
    }
}
