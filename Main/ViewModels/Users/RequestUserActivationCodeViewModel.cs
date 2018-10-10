using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.Users
{
    public class RequestUserActivationCodeViewModel
    {
        #region Properties

        /// <summary>
        /// email of account
        /// </summary>
        [Required]
        public string Email { get; set; }

        #endregion
    }
}
