using SystemDatabase.Models.Entities;

namespace Main.ViewModels.Accounts
{
    public class SearchAccountTokenResult
    {
        /// <summary>
        /// List of searched account.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// List of searched tokens.
        /// </summary>
        public Token Token { get; set; }
    }
}