﻿using System.Collections.Generic;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.PostCategorization
{
    public class SearchPostCategorizationViewModel
    {
        #region Properties

        /// <summary>
        /// Category id.
        /// </summary>
        public HashSet<int> CategoryIds { get; set; }

        /// <summary>
        /// Post id.
        /// </summary>
        public HashSet<int> PostIds { get; set; }

        /// <summary>
        /// Time when categorization was created.
        /// </summary>
        public Range<double?, double?> CategorizationTime { get; set; }

        /// <summary>
        /// Post categorization sort.
        /// </summary>
        public Sort<PostCategorizationSort> Sort { get; set; }

        /// <summary>
        /// Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}