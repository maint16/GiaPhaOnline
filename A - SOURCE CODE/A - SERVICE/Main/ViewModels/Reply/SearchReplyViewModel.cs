using System.Collections.Generic;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Models;
using Shared.Models;

namespace Main.ViewModels.Reply
{
    public class SearchReplyViewModel
    {

        #region Properties

        /// <summary>
        /// List of reply id
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// List of topic id
        /// </summary>
        public List<int> TopicIds { get; set; }

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
        /// List of reply content
        /// </summary>
        public List<string> Contents { get; set; }

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
        public Sort<ReplySort> Sort { get; set; }

        #endregion

    }
}
