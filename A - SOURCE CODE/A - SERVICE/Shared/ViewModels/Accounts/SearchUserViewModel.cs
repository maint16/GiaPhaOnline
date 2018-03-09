using System;
using System.Collections.Generic;
using System.Text;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Accounts
{
    public class SearchUserViewModel
    {
        /// <summary>
        ///     Id of account.
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// Sorted property & direction.
        /// </summary>
        public Sort<AccountSort> Sort { get; set; }

        /// <summary>
        ///     Pagination of records filter.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}
