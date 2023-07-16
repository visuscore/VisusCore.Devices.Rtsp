using OrchardCore.Modules.Manifest;
using ConsumerFeatureIds = VisusCore.Consumer.Constants.FeatureIds;
using DevicesFeatureIds = VisusCore.Devices.Constants.FeatureIds;
using EventBusFeatureIds = VisusCore.EventBus.Constants.FeatureIds;
using FfmpegFeatureIds = VisusCore.Ffmpeg.Constants.FeatureIds;
using StorageFeatureIds = VisusCore.Storage.Constants.FeatureIds;
using TenantHostedServiceFeatureIds = VisusCore.TenantHostedService.Constants.FeatureIds;

[assembly: Module(
    Name = "VisusCore Rtsp Devices",
    Author = "VisusCore",
    Version = "0.0.1",
    Description = "Rtsp video source.",
    Category = "VisusCore",
    Website = "https://github.com/visuscore/VisusCore.Devices.Rtsp",
    Dependencies = new[]
    {
        ConsumerFeatureIds.Module,
        DevicesFeatureIds.Module,
        EventBusFeatureIds.Module,
        FfmpegFeatureIds.Module,
        StorageFeatureIds.Module,
        TenantHostedServiceFeatureIds.Loader,
        "OrchardCore.Lists",
        "OrchardCore.Title",
    }
)]
