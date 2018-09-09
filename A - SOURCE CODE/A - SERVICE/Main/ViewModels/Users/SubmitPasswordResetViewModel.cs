using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.Users
{
    public class SubmitPasswordResetViewModel
    {
        #region Properties

        /// <summary>
        ///     Email which is used for account registration.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        ///     Password of account.
        /// </summary>
        [Required]
        public string Password { get; set; }

        #endregion
    }
}
