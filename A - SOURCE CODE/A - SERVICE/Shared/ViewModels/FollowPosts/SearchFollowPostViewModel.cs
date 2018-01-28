using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.FollowPosts
{
    public class SearchFollowPostViewModel
    {
        #region Properties

        /// <summary>
        /// Id of follower.
        /// </summary>
        public int? FollowerId { get; set; }

        /// <summary>
        /// Id of post.
        /// </summary>
        public int? PostId { get; set; }

        /// <summary>
        /// Follow post statuses.
        /// </summary>
        public HashSet<FollowPostStatus> Statuses { get; set; }

        /// <summary>
        /// Time when follow post was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// Sorted property & direction.
        /// </summary>
        public Sort<FollowPostSort> Sort { get; set; }

        /// <summary>
        /// Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}