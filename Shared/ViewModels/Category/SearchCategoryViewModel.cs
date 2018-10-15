using System.Collections.Generic;
using Shared.Enumerations;
using Shared.Enumerations.Order;
using Shared.Models;

namespace Shared.ViewModels.Category
{
    public class SearchCategoryViewModel
    {
        #region Properties

        /// <summary>
        ///     List of category id
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        ///     List of category group id
        /// </summary>
        public HashSet<int> CategoryGroupIds { get; set; }

        /// <summary>
        ///     List of creator id
        /// </summary>
        public HashSet<int> CreatorIds { get; set; }

        public HashSet<string> Names { get; set; }

        public HashSet<string> Descriptions { get; set; }

        /// <summary>
        ///     List of category status
        /// </summary>
        public HashSet<ItemStatus> Statuses { get; set; }

        /// <summary>
        ///     Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        ///     Sort property & direction
        /// </summary>
        public Sort<CategoriesSort> Sort { get; set; }

        #endregion
    }
}