using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Lists.Models;
using OrchardCore.Title.Models;
using VisusCore.Devices.Models;
using VisusCore.Devices.Rtsp.Constants;
using VisusCore.Devices.Rtsp.Models;

namespace VisusCore.Devices.Rtsp.Migrations;

public class RtspDeviceMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public RtspDeviceMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public int Create()
    {
        _contentDefinitionManager.AlterTypeDefinition(ContentTypeNames.RtspDevice, typeBuilder => typeBuilder
            .DisplayedAs("RTSP Device")
            .Creatable()
            .Listable()
            .WithPart(nameof(TitlePart))
            .WithPart(nameof(DevicePart))
            .WithPart(nameof(RtspDevicePart))
            .WithPart(nameof(ListPart), partBuilder => partBuilder
                .WithSettings(new ListPartSettings
                {
                    ContainedContentTypes = new[]
                    {
                        ContentTypeNames.RtspDeviceStream,
                    },
                })));

        return 1;
    }
}
