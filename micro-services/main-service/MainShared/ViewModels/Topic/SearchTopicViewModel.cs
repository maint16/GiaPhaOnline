using System.Collections.Generic;
using ClientShared.Enumerations;
using ClientShared.Enumerations.Order;
using ClientShared.Models;

namespace AppShared.ViewModels.Topic
{
    public class SearchTopicViewModel
    {
        #region Properties

        /// <summary>
        ///     List of topic id
        /// </summary>
        public HashSet<int> Ids { get; set; }

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
        ///     List of topic title
        /// </summary>
        public HashSet<string> Titles { get; set; }

        /// <summary>
        ///     List of topic body
        /// </summary>
        public HashSet<string> Bodies { get; set; }

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
        public Sort<TopicSort> Sort { get; set; }

        #endregion
    }
}