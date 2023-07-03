using System;
using System.ComponentModel.DataAnnotations;
using VisusCore.AidStack.Attributes;

namespace VisusCore.Devices.Rtsp.ViewModels;

public class RtspDeviceEditViewModel
{
    [Required]
    public string HostName { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int ConnectionTimeout { get; set; }
    public bool RequireCredentials { get; set; }
    [RequiredIf<bool>(PropertyName = nameof(RequireCredentials), PropertyValue = true)]
    public string UserName { get; set; }
    public string Password { get; set; }
}
