using OrchardCore.ContentManagement;
using System;
using VisusCore.AidStack.OrchardCore.Parts.Indexing;
using VisusCore.Devices.Rtsp.Core.Models;
using VisusCore.Devices.Rtsp.Models;

namespace VisusCore.Devices.Rtsp.Indexing;

public class RtspDevicePartIndexProvider : ContentPartIndexProvider<RtspDevicePart, RtspDevicePartIndex>
{
    public RtspDevicePartIndexProvider(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected override RtspDevicePartIndex CreateIndex(RtspDevicePart part, ContentItem contentItem) =>
        new()
        {
            ContentItemId = contentItem.ContentItemId,
            ContentItemVersionId = contentItem.ContentItemVersionId,
            ContentType = contentItem.ContentType,
            Latest = contentItem.Latest,
            Published = contentItem.Published,
            HostName = part.HostName,
            RequireCredentials = part.RequireCredentials,
            UserName = part.UserName,
            Password = part.Password,
            ConnectionTimeout = part.ConnectionTimeout,
        };
}
