using System.Linq;

namespace Shared.ViewModels.Token
{
    public class FindTokensResultViewModel
    {
        /// <summary>
        /// List of searched tokens.
        /// </summary>
        public IQueryable<SystemDatabase.Models.Entities.Token> Tokens { get; set; }

        /// <summary>
        /// Total tokens which have been searched.
        /// </summary>
        public int Total { get; set; }
    }
}