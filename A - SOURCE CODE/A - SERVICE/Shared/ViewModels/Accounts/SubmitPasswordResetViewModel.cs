using System.ComponentModel.DataAnnotations;
using Shared.Resources;

namespace Shared.ViewModels.Accounts
{
    public class SubmitPasswordResetViewModel
    {
        /// <summary>
        /// Password which is used for account.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(HttpValidationMessages), ErrorMessageResourceName = "InformationIsRequired")]
        public string Password { get; set; }

        /// <summary>
        /// Email which is for registering account to gain access into system.
        /// </summary>
        [Required]
        public string Email { get; set; }
    }
}