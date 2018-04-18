using SystemConstant.Enumerations;

namespace Shared.ViewModels.Accounts
{
    public class ChangeUserStatusViewModel
    {
        #region Properties

        /// <summary>
        /// Id of user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Status of account.
        /// </summary>
        public AccountStatus Status { get; set; }

        #endregion
    }
}