using OrchardCore.ContentManagement;
using System;
using VisusCore.AidStack.OrchardCore.Parts.Indexing;
using VisusCore.Devices.Rtsp.Core.Models;
using VisusCore.Devices.Rtsp.Models;

namespace VisusCore.Devices.Rtsp.Indexing;

public class RtspDeviceStreamPartIndexProvider : ContentPartIndexProvider<RtspDeviceStreamPart, RtspDeviceStreamPartIndex>
{
    public RtspDeviceStreamPartIndexProvider(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected override RtspDeviceStreamPartIndex CreateIndex(RtspDeviceStreamPart part, ContentItem contentItem) =>
        new()
        {
            ContentItemId = contentItem.ContentItemId,
            ContentItemVersionId = contentItem.ContentItemVersionId,
            ContentType = contentItem.ContentType,
            Latest = contentItem.Latest,
            Published = contentItem.Published,
            AllowAudio = part.AllowAudio,
            Path = part.Path,
            Port = part.Port,
            PreferTcp = part.PreferTcp,
            Type = part.Type,
        };
}
