using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using VisusCore.Devices.Rtsp.Models;

namespace VisusCore.Devices.Rtsp.Migrations;

public class RtspDeviceStreamPartMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public RtspDeviceStreamPartMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public int Create()
    {
        _contentDefinitionManager.AlterPartDefinition<RtspDeviceStreamPart>(definition => definition
            .Configure(definition => definition
                .Attachable()
                .WithDisplayName("RTSP Device Stream")));

        return 1;
    }
}
