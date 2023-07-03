using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;
using VisusCore.Devices.Rtsp.Models;
using VisusCore.Devices.Rtsp.ViewModels;

namespace VisusCore.Devices.Rtsp.Drivers;

public class RtspDeviceStreamDisplayDriver : ContentPartDisplayDriver<RtspDeviceStreamPart>
{
    public override IDisplayResult Edit(RtspDeviceStreamPart part, BuildPartEditorContext context) =>
        Initialize<RtspDeviceStreamEditViewModel>(GetEditorShapeType(context), viewModel =>
        {
            viewModel.Enabled = part.Enabled;
            viewModel.Path = part.Path;
            viewModel.Port = part.Port;
            viewModel.PreferTcp = part.PreferTcp;
            viewModel.Type = part.Type;
        });

    public override async Task<IDisplayResult> UpdateAsync(
        RtspDeviceStreamPart part,
        IUpdateModel updater,
        UpdatePartEditorContext context)
    {
        var viewModel = new RtspDeviceStreamEditViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Enabled = viewModel.Enabled;
        part.Path = viewModel.Path;
        part.Port = viewModel.Port;
        part.PreferTcp = viewModel.PreferTcp;
        part.Type = viewModel.Type;

        return await EditAsync(part, context);
    }
}
