using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using VisusCore.AidStack.OrchardCore.Extensions;
using VisusCore.Devices.Rtsp.Core.Events;
using VisusCore.Devices.Rtsp.Core.Models;
using VisusCore.Devices.Rtsp.Drivers;
using VisusCore.Devices.Rtsp.Handlers;
using VisusCore.Devices.Rtsp.Indexing;
using VisusCore.Devices.Rtsp.Migrations;
using VisusCore.Devices.Rtsp.Models;
using VisusCore.Devices.Rtsp.Services;
using VisusCore.EventBus.Core.Extensions;
using VisusCore.TenantHostedService.Core.Extensions;

namespace VisusCore.Devices.Rtsp;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<RtspDeviceStreamPartMigrations>();
        services.AddDataMigration<RtspDeviceStreamPartIndexMigrations>();
        services.AddScopedContentPartIndexProvider<
            RtspDeviceStreamPartIndexProvider,
            RtspDeviceStreamPart,
            RtspDeviceStreamPartIndex>();
        services.AddContentPart<RtspDeviceStreamPart>()
            .UseDisplayDriver<RtspDeviceStreamDisplayDriver>();
        services.AddDataMigration<RtspDeviceStreamMigrations>();

        services.AddDataMigration<RtspDevicePartMigrations>();
        services.AddDataMigration<RtspDevicePartIndexMigrations>();
        services.AddScopedContentPartIndexProvider<
            RtspDevicePartIndexProvider,
            RtspDevicePart,
            RtspDevicePartIndex>();
        services.AddContentPart<RtspDevicePart>()
            .UseDisplayDriver<RtspDeviceDisplayDriver>();
        services.AddDataMigration<RtspDeviceMigrations>();

        services.AddTenantHostedScopedService<RtspConnectionManager>();

        services.AddScoped<IContentHandler, RtspDeviceConfigurationChangeHandler>();
        services.AddSingleton<RtspDeviceConfigurationChangeListener>();
        services.AddSingleton<RtspDeviceStreamConfigurationChangeListener>();
        services.AddReactiveEventConsumer<RtspDevicePublishedEvent>();
        services.AddReactiveEventConsumer<RtspDeviceRemovedEvent>();
        services.AddReactiveEventConsumer<RtspDeviceUnpublishedEvent>();
        services.AddReactiveEventConsumer<RtspDeviceUpdatedEvent>();
        services.AddReactiveEventConsumer<RtspDeviceStreamPublishedEvent>();
        services.AddReactiveEventConsumer<RtspDeviceStreamRemovedEvent>();
        services.AddReactiveEventConsumer<RtspDeviceStreamUnpublishedEvent>();
        services.AddReactiveEventConsumer<RtspDeviceStreamUpdatedEvent>();
    }
}
