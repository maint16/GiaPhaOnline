using System.Collections.Generic;
using Shared.Enumerations;
using Shared.Enumerations.Order;
using Shared.Models;

namespace Shared.ViewModels.CategoryGroup
{
    public class SearchCategoryGroupViewModel
    {
        #region Properties

        /// <summary>
        ///     List of category group id
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        ///     List of creator id
        /// </summary>
        public HashSet<int> CreatorIds { get; set; }

        /// <summary>
        ///     List of category group name
        /// </summary>
        public HashSet<string> Names { get; set; }

        /// <summary>
        ///     List of category group description
        /// </summary>
        public HashSet<string> Descriptions { get; set; }

        /// <summary>
        ///     List of category group status
        /// </summary>
        public HashSet<ItemStatus> Statuses { get; set; }

        /// <summary>
        ///     Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        ///     Sort property & direction
        /// </summary>
        public Sort<CategoryGroupSort> Sort { get; set; }

        #endregion
    }
}