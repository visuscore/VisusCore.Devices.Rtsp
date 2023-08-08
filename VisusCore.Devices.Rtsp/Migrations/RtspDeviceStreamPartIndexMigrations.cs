using Lombiq.HelpfulLibraries.OrchardCore.Data;
using OrchardCore.Data.Migration;
using VisusCore.Devices.Rtsp.Core.Models;
using YesSql.Sql;

namespace VisusCore.Devices.Rtsp.Migrations;

public class RtspDeviceStreamPartIndexMigrations : DataMigration
{
    public int Create()
    {
        SchemaBuilder.CreateMapIndexTable<RtspDeviceStreamPartIndex>(table => table
            .MapContentPartIndex()
            .Column(model => model.Port)
            .Column(model => model.PreferTcp)
            .Column(model => model.Path, column => column.Nullable().WithLength(1024))
            .Column(model => model.Type, column => column.WithLength(255))
            .Column(model => model.AllowAudio));

        return 1;
    }
}
