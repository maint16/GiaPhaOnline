using System.Collections.Generic;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Models;
using Shared.Models;

namespace Main.ViewModels.FollowCategory
{
    public class SearchFollowCategoryViewModel
    {
        #region Properties

        /// <summary>
        /// List of follower id
        /// </summary>
        public List<int> FollowerIds { get; set; }

        /// <summary>
        /// List of category id
        /// </summary>
        public List<int> CategoryIds { get; set; }

        /// <summary>
        /// List of follow category status
        /// </summary>
        public List<FollowStatus> Statuses { get; set; }

        /// <summary>
        /// Created date.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        /// Sort property & direction
        /// </summary>
        public Sort<FollowCategorySort> Sort { get; set; }

        #endregion
    }
}
