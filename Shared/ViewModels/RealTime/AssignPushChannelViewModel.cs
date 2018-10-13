using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.RealTime
{
    public class AssignPushChannelViewModel
    {
        #region Properties

        /// <summary>
        ///     Connection id of client.
        /// </summary>
        [Required]
        public string DeviceId { get; set; }

        #endregion
    }
}