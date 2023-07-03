using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using VisusCore.Devices.Rtsp.Models;

namespace VisusCore.Devices.Rtsp.Migrations;

public class RtspDevicePartMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public RtspDevicePartMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public int Create()
    {
        _contentDefinitionManager.AlterPartDefinition<RtspDevicePart>(definition => definition
            .Configure(definition => definition
                .Attachable()
                .WithDisplayName("RTSP Device")));

        return 1;
    }
}
