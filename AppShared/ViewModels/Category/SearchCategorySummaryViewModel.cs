using System.Collections.Generic;
using ClientShared.Models;

namespace AppShared.ViewModels.Category
{
    public class SearchCategorySummaryViewModel
    {
        #region Properties

        /// <summary>
        ///     Category indexes.
        /// </summary>
        public HashSet<int> CategoryIds { get; set; }

        /// <summary>
        ///     Pagination.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion

        #region Constructor

        public SearchCategorySummaryViewModel()
        {
        }

        public SearchCategorySummaryViewModel(HashSet<int> categoryIds)
        {
            CategoryIds = categoryIds;
        }

        public SearchCategorySummaryViewModel(HashSet<int> categoryIds, Pagination pagination)
        {
            CategoryIds = categoryIds;
            Pagination = pagination;
        }

        #endregion
    }
}