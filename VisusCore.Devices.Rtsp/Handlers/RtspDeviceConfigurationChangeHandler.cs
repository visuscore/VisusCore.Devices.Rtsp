using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Lists.Models;
using System;
using System.Threading.Tasks;
using Tingle.EventBus;
using VisusCore.Configuration.VideoStream.Core.Models;
using VisusCore.Devices.Models;
using VisusCore.Devices.Rtsp.Core.Events;
using VisusCore.Devices.Rtsp.Models;

namespace VisusCore.Devices.Rtsp.Handlers;

public class RtspDeviceConfigurationChangeHandler : ContentHandlerBase
{
    private readonly IEventPublisher _eventPublisher;

    public RtspDeviceConfigurationChangeHandler(IEventPublisher eventPublisher) =>
        _eventPublisher = eventPublisher;

    public override Task PublishedAsync(PublishContentContext context) =>
        PublishEventConditionallyAsync(context ?? throw new ArgumentNullException(nameof(context)));

    public override Task RemovedAsync(RemoveContentContext context) =>
        PublishEventConditionallyAsync(context ?? throw new ArgumentNullException(nameof(context)));

    public override Task UnpublishedAsync(PublishContentContext context) =>
        PublishEventConditionallyAsync(context ?? throw new ArgumentNullException(nameof(context)), unpublish: true);

    public override Task UpdatedAsync(UpdateContentContext context) =>
        PublishEventConditionallyAsync(context ?? throw new ArgumentNullException(nameof(context)));

    private async Task PublishEventConditionallyAsync(ContentContextBase context, bool unpublish = false)
    {
        if (!(context.ContentItem.Has<DevicePart>() && context.ContentItem.Has<RtspDevicePart>())
            && !(
                context.ContentItem.Has<RtspDeviceStreamPart>()
                && context.ContentItem.Has<StreamEntityPart>()
                && context.ContentItem.Has<ContainedPart>()))
        {
            return;
        }

        if (context.ContentItem.Has<DevicePart>() && context.ContentItem.Has<RtspDevicePart>())
        {
            switch (context)
            {
                case PublishContentContext when unpublish:
                    await _eventPublisher.PublishAsync(new RtspDeviceUnpublishedEvent(context.ContentItem.ContentItemId));
                    break;
                case PublishContentContext when !unpublish:
                    await _eventPublisher.PublishAsync(new RtspDevicePublishedEvent(context.ContentItem.ContentItemId));
                    break;
                case RemoveContentContext:
                    await _eventPublisher.PublishAsync(new RtspDeviceRemovedEvent(context.ContentItem.ContentItemId));
                    break;
                case UpdateContentContext:
                    await _eventPublisher.PublishAsync(new RtspDeviceUpdatedEvent(context.ContentItem.ContentItemId));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported {nameof(context)} type '{context.GetType().FullName}'.");
            }

            return;
        }

        var containedPart = context.ContentItem.As<ContainedPart>();
        switch (context)
        {
            case PublishContentContext when unpublish:
                await _eventPublisher.PublishAsync(
                    new RtspDeviceStreamUnpublishedEvent(containedPart.ListContentItemId, context.ContentItem.ContentItemId));
                break;
            case PublishContentContext when !unpublish:
                await _eventPublisher.PublishAsync(
                    new RtspDeviceStreamPublishedEvent(containedPart.ListContentItemId, context.ContentItem.ContentItemId));
                break;
            case RemoveContentContext:
                await _eventPublisher.PublishAsync(
                    new RtspDeviceStreamRemovedEvent(containedPart.ListContentItemId, context.ContentItem.ContentItemId));
                break;
            case UpdateContentContext:
                await _eventPublisher.PublishAsync(
                    new RtspDeviceStreamUpdatedEvent(containedPart.ListContentItemId, context.ContentItem.ContentItemId));
                break;
            default:
                throw new InvalidOperationException($"Unsupported {nameof(context)} type '{context.GetType().FullName}'.");
        }
    }
}
