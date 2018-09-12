using System.Collections.Generic;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Models;
using Shared.Models;

namespace Main.ViewModels.Topic
{
    public class SearchTopicViewModel
    {
        #region Properties

        /// <summary>
        /// List of topic id
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// List of category id
        /// </summary>
        public List<int> CategoryIds { get; set; }

        /// <summary>
        /// List of category group id
        /// </summary>
        public List<int> CategoryGroupIds { get; set; }

        /// <summary>
        /// List of owner id
        /// </summary>
        public List<int> OwnerIds { get; set; }

        /// <summary>
        /// List of topic title
        /// </summary>
        public List<string> Titles { get; set; }

        /// <summary>
        /// List of topic body
        /// </summary>
        public List<string> Bodies { get; set; }

        /// <summary>
        /// List of category status
        /// </summary>
        public List<ItemStatus> Statuses { get; set; }

        /// <summary>
        /// Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        /// Sort property & direction
        /// </summary>
        public Sort<TopicSort> Sort { get; set; }

        #endregion
    }
}
