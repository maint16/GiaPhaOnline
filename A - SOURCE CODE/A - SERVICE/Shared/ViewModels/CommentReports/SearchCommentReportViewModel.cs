using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.CommentReports
{
    public class SearchCommentReportViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of comment which should be reported.
        /// </summary>
        public int? CommentId { get; set; }

        /// <summary>
        ///     Id of comment reporter.
        /// </summary>
        public int? ReporterId { get; set; }

        /// <summary>
        ///     Id of post which comment belongs to.
        /// </summary>
        public int? PostId { get; set; }

        /// <summary>
        ///     Onwer index which owns the post.
        /// </summary>
        public int? OwnerId { get; set; }

        /// <summary>
        ///     Reason of report.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        ///  Status of comment report.
        /// </summary>
        public HashSet<CommentReportStatus> Statuses { get; set; }
        
        public Sort<CommentReportSort> Sort { get; set; }

        /// <summary>
        ///     When the comment should be created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// When the comment report was lastly modified.
        /// </summary>
        public Range<double?, double?> LastModifiedTime { get; set; }

        /// <summary>
        ///     Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}