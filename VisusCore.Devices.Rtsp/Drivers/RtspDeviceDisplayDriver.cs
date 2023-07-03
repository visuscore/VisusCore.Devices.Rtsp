using Microsoft.AspNetCore.DataProtection;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;
using VisusCore.Devices.Rtsp.Models;
using VisusCore.Devices.Rtsp.ViewModels;

namespace VisusCore.Devices.Rtsp.Drivers;

public class RtspDeviceDisplayDriver : ContentPartDisplayDriver<RtspDevicePart>
{
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public RtspDeviceDisplayDriver(IDataProtectionProvider dataProtectionProvider) =>
        _dataProtectionProvider = dataProtectionProvider;

    public override IDisplayResult Edit(RtspDevicePart part, BuildPartEditorContext context) =>
        Initialize<RtspDeviceEditViewModel>(GetEditorShapeType(context), viewModel =>
        {
            viewModel.RequireCredentials = part.RequireCredentials;
            viewModel.HostName = part.HostName;
            viewModel.UserName = part.UserName;
            viewModel.Password = part.Password;
            viewModel.ConnectionTimeout = part.ConnectionTimeout;
        });

    public override async Task<IDisplayResult> UpdateAsync(
        RtspDevicePart part,
        IUpdateModel updater,
        UpdatePartEditorContext context)
    {
        var viewModel = new RtspDeviceEditViewModel();
        var previousPassword = part.Password;

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        if (string.IsNullOrEmpty(viewModel.Password))
        {
            part.Password = previousPassword;
        }
        else
        {
            var protector = _dataProtectionProvider.CreateProtector(nameof(RtspDevicePart));
            part.Password = protector.Protect(viewModel.Password);
        }

        part.HostName = viewModel.HostName;
        part.UserName = viewModel.UserName;
        part.RequireCredentials = viewModel.RequireCredentials;
        part.ConnectionTimeout = viewModel.ConnectionTimeout;

        return await EditAsync(part, context);
    }
}
