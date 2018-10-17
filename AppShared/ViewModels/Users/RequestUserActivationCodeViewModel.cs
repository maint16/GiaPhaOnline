using System.ComponentModel.DataAnnotations;

namespace AppShared.ViewModels.Users
{
    public class RequestUserActivationCodeViewModel
    {
        #region Properties

        /// <summary>
        ///     email of account
        /// </summary>
        [Required]
        public string Email { get; set; }

        #endregion
    }
}