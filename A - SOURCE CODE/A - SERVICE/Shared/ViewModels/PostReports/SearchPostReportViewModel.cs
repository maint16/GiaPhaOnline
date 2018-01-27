using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.PostReports
{
    public class SearchPostReportViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of post which is reported.
        /// </summary>
        public int? PostId { get; set; }

        /// <summary>
        ///     Id of post owner.
        /// </summary>
        public int? ReporterId { get; set; }

        /// <summary>
        ///     Id of report.
        /// </summary>
        public int? OwnerId { get; set; }

        /// <summary>
        ///     Post report body.
        /// </summary>
        public string Reason { get; set; }
        
        /// <summary>
        /// Post report statuses.
        /// </summary>
        public HashSet<PostReportStatus> Statuses { get; set; }

        /// <summary>
        ///     When the report was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// When the post report was lastly modified.
        /// </summary>
        public Range<double?, double?> LastModifiedTime { get; set; }

        /// <summary>
        /// Sort property & direction.
        /// </summary>
        public Sort<PostReportSort> Sort { get; set; }

        /// <summary>
        ///     Result pagination.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion

    }
}