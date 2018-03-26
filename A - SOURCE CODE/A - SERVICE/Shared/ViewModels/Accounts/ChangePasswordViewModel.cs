using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.Accounts
{
    public class ChangePasswordViewModel
    {
        #region Properties

        /// <summary>
        /// Current password.
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Password which will be applied to account.
        /// </summary>
        [Required]
        public string NewPassword { get; set; }

        #endregion
    }
}