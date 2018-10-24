using System.Collections.Generic;
using ClientShared.Enumerations;
using ClientShared.Enumerations.Order;
using ClientShared.Models;

namespace AppShared.ViewModels.ReportTopic
{
    public class SearchReportTopicViewModel
    {
        #region Methods

        /// <summary>
        ///     List of topic id
        /// </summary>
        public HashSet<int> TopicIds { get; set; }

        /// <summary>
        ///     List of reporter id
        /// </summary>
        public HashSet<int> ReporterIds { get; set; }

        /// <summary>
        ///     List of owner topic id
        /// </summary>
        public HashSet<int> OwnerIds { get; set; }

        /// <summary>
        ///     List of reason
        /// </summary>
        public HashSet<string> Reasons { get; set; }

        /// <summary>
        ///     List of report topic status
        /// </summary>
        public HashSet<ItemStatus> Statuses { get; set; }

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
        public Sort<ReportTopicSort> Sort { get; set; }

        #endregion
    }
}