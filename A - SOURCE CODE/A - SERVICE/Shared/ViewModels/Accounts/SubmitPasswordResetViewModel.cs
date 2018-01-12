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
    }
}