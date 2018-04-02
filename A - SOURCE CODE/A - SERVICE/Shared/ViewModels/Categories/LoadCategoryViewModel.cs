using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Categories
{
    public class LoadCategoryViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of Category.
        /// </summary>
        [Required]
        [MinLength(1)]
        public List<int> Ids { get; set; }

        /// <summary>
        /// Sorted property & direction.
        /// </summary>
        public Sort<CategoriesSort> Sort { get; set; }

        /// <summary>
        ///     Pagination of records filter.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}
