namespace Shared.ViewModels.Accounts
{
    public class LoginViewModel
    {
        #region Properties

        /// <summary>
        /// Email which is used for logging into system.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password which is for logging into system.
        /// </summary>
        public string Password { get; set; }

        #endregion
    }
}