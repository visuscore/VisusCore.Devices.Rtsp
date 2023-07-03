using System.ComponentModel.DataAnnotations;
using VisusCore.Devices.Rtsp.Core.Models;

namespace VisusCore.Devices.Rtsp.ViewModels;

public class RtspDeviceStreamEditViewModel
{
    public bool Enabled { get; set; }
    [Required]
    [Range(1, 65535)]
    public int Port { get; set; }
    public bool PreferTcp { get; set; }
    public string Path { get; set; }
    [Required]
    public EStreamType Type { get; set; }
    public bool AllowAudio { get; set; }
}
