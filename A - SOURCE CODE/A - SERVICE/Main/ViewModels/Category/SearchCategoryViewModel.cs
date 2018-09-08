using System.Collections.Generic;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Models;
using Shared.Models;

namespace Main.ViewModels.Category
{
    public class SearchCategoryViewModel
    {
        #region Properties

        /// <summary>
        /// List of category id
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// List of category group id
        /// </summary>
        public List<int> CategoryGroupIds { get; set; }

        /// <summary>
        /// List of creator id
        /// </summary>
        public List<int> CreatorIds { get; set; }

        /// <summary>
        /// List of category name
        /// </summary>
        public List<string> Names { get; set; }

        /// <summary>
        /// List of category description
        /// </summary>
        public List<string> Descriptions { get; set; }

        /// <summary>
        /// List of category status
        /// </summary>
        public List<ItemStatus> Statuses { get; set; }

        /// <summary>
        /// Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        /// Sort property & direction
        /// </summary>
        public Sort<CategoriesSort> Sort { get; set; }

        #endregion
    }
}
