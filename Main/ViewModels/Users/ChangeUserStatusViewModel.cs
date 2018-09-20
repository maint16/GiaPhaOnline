using AppModel.Enumerations;

namespace Main.ViewModels.Users
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
