using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Token
{
    public class FindTokensViewModel
    {
        /// <summary>
        /// Who owns the token.
        /// </summary>
        public int [] OwnerIndexes { get; set; }

        /// <summary>
        /// Types of token.
        /// </summary>
        public TokenType [] Types { get; set; }

        /// <summary>
        /// Token code.
        /// </summary>
        public TextSearch Code { get; set; }

        /// <summary>
        /// When the token was issued.
        /// </summary>
        public Range<double?, double?> Issued { get; set; }

        /// <summary>
        /// When the token should be expired.
        /// </summary>
        public Range<double?, double?> Expired { get; set; }

        /// <summary>
        /// Sort property & direction.
        /// </summary>
        public Sort<TokenSort> Sort { get; set; }
        
        /// <summary>
        /// Token search pagination.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}