﻿using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Categories
{
    public class SearchCategoryViewModel
    {
        /// <summary>
        ///     Id of category.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Id of creator.
        /// </summary>
        public int? CreatorId { get; set; }

        /// <summary>
        ///     Name of category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     When the category was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        ///     When the category was lastly modified.
        /// </summary>
        public Range<double?, double?> LastModifiedTime { get; set; }

        /// <summary>
        ///     Which property & direction should be used for sorting categories.
        /// </summary>
        public Sort<CategoriesSort> Sort { get; set; }
        
        /// <summary>
        ///     Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}