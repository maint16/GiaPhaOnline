using AppDb.Models.Entities;

namespace Main.ViewModels.Users
{
    public class SearchAccountTokenResult
    {
        #region Properties

        /// <summary>
        /// Token of account
        /// </summary>
        public AccessToken Token { get; set; }

        /// <summary>
        /// Account information
        /// </summary>
        public Account Account { get; set; }

        #endregion
    }
}
