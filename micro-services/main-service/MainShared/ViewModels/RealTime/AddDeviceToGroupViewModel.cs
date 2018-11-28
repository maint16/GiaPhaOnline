using System.ComponentModel.DataAnnotations;

namespace MainShared.ViewModels.RealTime
{
#if DEBUG
    public class AddDeviceToGroupViewModel
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string Group { get; set; }
    }
#endif
}