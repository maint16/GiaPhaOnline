using System.ComponentModel.DataAnnotations;
using SystemConstant.Enumerations;

namespace Shared.ViewModels.Accounts
{
    public class ChangeUserStatusViewModel
    {
        #region Properties

        /// <summary>
        /// Reason of change.
        /// </summary>
        [Required]
        public string Reason { get; set; }

        /// <summary>
        /// Status of account.
        /// </summary>
        public AccountStatus Status { get; set; }

        #endregion
    }
}