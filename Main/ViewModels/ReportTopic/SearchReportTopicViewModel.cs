using System.Collections.Generic;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Models;
using Shared.Models;

namespace Main.ViewModels.ReportTopic
{
    public class SearchReportTopicViewModel
    {
        #region Methods

        /// <summary>
        /// List of topic id
        /// </summary>
        public List<int> TopicIds { get; set; }

        /// <summary>
        /// List of reporter id
        /// </summary>
        public List<int> ReporterIds { get; set; }

        /// <summary>
        /// List of owner topic id
        /// </summary>
        public List<int> OwnerIds { get; set; }

        /// <summary>
        /// List of reason
        /// </summary>
        public List<string> Reasons { get; set; }

        /// <summary>
        /// List of report topic status
        /// </summary>
        public List<ItemStatus> Statuses { get; set; }

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
        public Sort<ReportTopicSort> Sort { get; set; }

        #endregion
    }
}
