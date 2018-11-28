using System.ComponentModel.DataAnnotations;
using MainShared.Resources;

namespace MainShared.ViewModels
{
    public class AddDeviceViewModel
    {
        #region Properties

        /// <summary>
        ///     Device id which is returned from push notification sdk.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(HttpValidationMessages),
            ErrorMessageResourceName = "InformationIsRequired")]
        public string DeviceId { get; set; }

        #endregion
    }
}