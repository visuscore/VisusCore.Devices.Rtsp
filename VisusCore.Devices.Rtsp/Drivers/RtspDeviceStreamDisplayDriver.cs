using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System;
using System.Threading.Tasks;
using VisusCore.Devices.Rtsp.Models;
using VisusCore.Devices.Rtsp.ViewModels;

namespace VisusCore.Devices.Rtsp.Drivers;

public class RtspDeviceStreamDisplayDriver : ContentPartDisplayDriver<RtspDeviceStreamPart>
{
    public override IDisplayResult Edit(RtspDeviceStreamPart part, BuildPartEditorContext context) =>
        Initialize<RtspDeviceStreamEditViewModel>(GetEditorShapeType(context), viewModel =>
        {
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
        if (part is null)
        {
            throw new ArgumentNullException(nameof(part));
        }

        if (updater is null)
        {
            throw new ArgumentNullException(nameof(updater));
        }

        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var viewModel = new RtspDeviceStreamEditViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Path = viewModel.Path;
        part.Port = viewModel.Port;
        part.PreferTcp = viewModel.PreferTcp;
        part.Type = viewModel.Type;

        return await EditAsync(part, context);
    }
}
