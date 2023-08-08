using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Data.Migration;
using OrchardCore.Title.Models;
using VisusCore.Configuration.VideoStream.Core.Models;
using VisusCore.Devices.Rtsp.Constants;
using VisusCore.Devices.Rtsp.Models;
using VisusCore.Storage.Models;

namespace VisusCore.Devices.Rtsp.Migrations;

public class RtspDeviceStreamMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public RtspDeviceStreamMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public int Create()
    {
        _contentDefinitionManager.AlterTypeDefinition(ContentTypeNames.RtspDeviceStream, typeBuilder => typeBuilder
            .DisplayedAs("RTSP Device Stream")
            .WithPart(nameof(TitlePart), part => part
                .WithSettings(new TitlePartSettings
                {
                    Pattern = "{{ ContentItem.Content.StreamEntityPart.Name }}",
                    Options = TitlePartOptions.GeneratedHidden,
                }))
            .WithPart(nameof(StreamEntityPart))
            .WithPart(nameof(RtspDeviceStreamPart))
            .WithPart(nameof(StreamStorageProviderPart))
            .WithPart(nameof(StreamStorageModePart))
            .WithPart(nameof(StreamStorageSizeLimitPart))
            .WithPart(nameof(StreamStorageTimeLimitPart)));

        return 1;
    }
}
