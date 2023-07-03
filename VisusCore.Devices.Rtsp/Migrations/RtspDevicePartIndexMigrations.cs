using Lombiq.HelpfulLibraries.OrchardCore.Data;
using OrchardCore.Data.Migration;
using VisusCore.Devices.Rtsp.Core.Models;
using YesSql.Sql;

namespace VisusCore.Devices.Rtsp.Migrations;

public class RtspDevicePartIndexMigrations : DataMigration
{
    public int Create()
    {
        SchemaBuilder.CreateMapIndexTable<RtspDevicePartIndex>(table => table
            .MapContentPartIndex()
            .Column(model => model.HostName, column => column.WithLength(253))
            .Column(model => model.ConnectionTimeout)
            .Column(model => model.RequireCredentials)
            .Column(model => model.UserName, column => column.Nullable().WithLength(255))
            .Column(model => model.Password, column => column.Nullable().WithLength(1024)));

        return 1;
    }
}
