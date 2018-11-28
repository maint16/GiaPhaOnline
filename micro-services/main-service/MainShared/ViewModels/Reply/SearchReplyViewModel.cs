using System.Collections.Generic;
using ClientShared.Enumerations;
using ClientShared.Enumerations.Order;
using ClientShared.Models;

namespace MainShared.ViewModels.Reply
{
    public class SearchReplyViewModel
    {
        #region Properties

        /// <summary>
        ///     List of reply id
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        ///     List of topic id
        /// </summary>
        public HashSet<int> TopicIds { get; set; }

        /// <summary>
        ///     List of category id
        /// </summary>
        public HashSet<int> CategoryIds { get; set; }

        /// <summary>
        ///     List of category group id
        /// </summary>
        public HashSet<int> CategoryGroupIds { get; set; }

        /// <summary>
        ///     List of owner id
        /// </summary>
        public HashSet<int> OwnerIds { get; set; }

        /// <summary>
        ///     List of reply content
        /// </summary>
        public HashSet<string> Contents { get; set; }

        /// <summary>
        ///     List of category status
        /// </summary>
        public HashSet<ItemStatus> Statuses { get; set; }

        /// <summary>
        ///     Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        ///     Sort property & direction
        /// </summary>
        public Sort<ReplySort> Sort { get; set; }

        #endregion
    }
}