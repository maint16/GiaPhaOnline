using System;
using System.Collections.Generic;
using System.Text;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.PostCategorization
{
    public class CountPostCategorizationViewModel
    {
        #region Properties

        /// <summary>
        /// Category id.
        /// </summary>
        public HashSet<int> CategoryIds { get; set; }

        /// <summary>
        /// Sorted property & direction.
        /// </summary>
        public Sort<PostCategorizationSort> Sort { get; set; }

        /// <summary>
        ///     Pagination of records filter.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}
