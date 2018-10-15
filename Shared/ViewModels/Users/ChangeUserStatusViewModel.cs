using Shared.Enumerations;

namespace Shared.ViewModels.Users
{
    public class ChangeUserStatusViewModel
    {
        #region Properties

        /// <summary>
        ///     Account status in the system.
        /// </summary>
        public UserStatus Status { get; set; }

        #endregion
    }
}