using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.Users
{
    public class ChangePasswordViewModel
    {
        #region Properties

        /// <summary>
        /// Old password
        /// </summary>
        [Required]
        public string OriginalPassword { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        [Required]
        public string Password { get; set; }

        #endregion
    }
}
