using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.FollowCategories
{
    public class SearchFollowCategoryViewModel
    {
        #region Properties

        /// <summary>
        /// Id of follower.
        /// </summary>
        public int? FollowerId { get; set; }

        /// <summary>
        /// Category id.
        /// </summary>
        public HashSet<int> CategoryIds { get; set; }

        /// <summary>
        /// Statuses of follow category.
        /// </summary>
        public HashSet<ItemStatus> Statuses { get; set; }

        /// <summary>
        /// Time when follow category was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// Property & direction to sort.
        /// </summary>
        public Sort<FollowCategorySort> Sort { get; set; }

        /// <summary>
        /// Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}