using Shared.Enumerations.Order;
using Shared.Models;

namespace Shared.ViewModels.CommentReports
{
    public class SearchCommentReportViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of comment which should be reported.
        /// </summary>
        public int? CommentIndex { get; set; }

        /// <summary>
        ///     Id of comment owner.
        /// </summary>
        public int? CommentOwnerIndex { get; set; }

        /// <summary>
        ///     Id of person who created comment report.
        /// </summary>
        public int? CommentReporterIndex { get; set; }

        /// <summary>
        ///     Body of comment which is reported.
        /// </summary>
        public TextSearch Body { get; set; }

        /// <summary>
        ///     Reason of report.
        /// </summary>
        public TextSearch Reason { get; set; }

        /// <summary>
        /// Which property should be sorted.
        /// </summary>
        public CommentReportSort Sort { get; set; }

        /// <summary>
        /// Sort direction.
        /// </summary>
        public SortDirection Direction { get; set; }

        /// <summary>
        ///     When the comment should be created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        ///     Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}