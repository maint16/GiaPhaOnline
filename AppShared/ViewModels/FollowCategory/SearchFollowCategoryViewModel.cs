using System.Collections.Generic;
using AppShared.Enumerations;
using AppShared.Enumerations.Order;
using AppShared.Models;

namespace AppShared.ViewModels.FollowCategory
{
    public class SearchFollowCategoryViewModel
    {
        #region Properties

        /// <summary>
        ///     List of follower id
        /// </summary>
        public HashSet<int> FollowerIds { get; set; }

        /// <summary>
        ///     List of category id
        /// </summary>
        public HashSet<int> CategoryIds { get; set; }

        /// <summary>
        ///     List of follow category status
        /// </summary>
        public HashSet<FollowStatus> Statuses { get; set; }

        /// <summary>
        ///     Created date.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        ///     Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        ///     Sort property & direction
        /// </summary>
        public Sort<FollowCategorySort> Sort { get; set; }

        #endregion
    }
}