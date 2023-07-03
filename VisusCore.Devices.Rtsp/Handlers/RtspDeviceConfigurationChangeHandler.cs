using OrchardCore.ContentManagement.Handlers;
using System.Threading.Tasks;
using VisusCore.Devices.Rtsp.Services;
using VisusCore.TenantHostedService.Abstractions.Services;

namespace VisusCore.Devices.Rtsp.Handlers;

public class RtspDeviceConfigurationChangeHandler : ContentHandlerBase
{
    private readonly ITenantHostedScopedServiceAccessor<RtspConnectionManager> _rtspConnectionManager;

    public RtspDeviceConfigurationChangeHandler(ITenantHostedScopedServiceAccessor<RtspConnectionManager> rtspConnectionManager) =>
        _rtspConnectionManager = rtspConnectionManager;

    public override Task PublishedAsync(PublishContentContext context) =>
        _rtspConnectionManager.Service.PostChangeNotificationAsync(context);

    public override Task RemovedAsync(RemoveContentContext context) =>
        _rtspConnectionManager.Service.PostChangeNotificationAsync(context);

    public override Task UnpublishedAsync(PublishContentContext context) =>
        _rtspConnectionManager.Service.PostChangeNotificationAsync(context);

    public override Task UpdatedAsync(UpdateContentContext context) =>
        _rtspConnectionManager.Service.PostChangeNotificationAsync(context);
}
