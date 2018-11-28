using System.Collections.Generic;
using ClientShared.Enumerations;
using ClientShared.Enumerations.Order;
using ClientShared.Models;

namespace AppShared.ViewModels.FollowTopic
{
    public class SearchFollowTopicViewModel
    {
        #region Properties

        /// <summary>
        ///     List of follower id
        /// </summary>
        public HashSet<int> FollowerIds { get; set; }

        /// <summary>
        ///     List of topic id
        /// </summary>
        public HashSet<int> TopicIds { get; set; }

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
        public Sort<FollowTopicSort> Sort { get; set; }

        #endregion
    }
}