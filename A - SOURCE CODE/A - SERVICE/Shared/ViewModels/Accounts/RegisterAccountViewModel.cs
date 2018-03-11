using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.Accounts
{
    public class RegisterAccountViewModel
    {
        #region Properties

        /// <summary>
        /// Email which is for registering account to gain access into system.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Password which is related to email.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Nick name of registered person.
        /// </summary>
        [Required]
        public string Nickname { get; set; }

        #endregion
    }
}