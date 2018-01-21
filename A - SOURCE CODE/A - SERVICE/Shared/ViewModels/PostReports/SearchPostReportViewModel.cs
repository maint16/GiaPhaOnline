using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.PostReports
{
    public class SearchPostReportViewModel
    {
        /// <summary>
        ///     Id of post report.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Id of post which is reported.
        /// </summary>
        public int? PostIndex { get; set; }

        /// <summary>
        ///     Id of post owner.
        /// </summary>
        public int? PostOwnerIndex { get; set; }

        /// <summary>
        ///     Id of report.
        /// </summary>
        public int? PostReporterIndex { get; set; }

        /// <summary>
        ///     Post report body.
        /// </summary>
        public TextSearch Body { get; set; }

        /// <summary>
        ///     Post report reason.
        /// </summary>
        public TextSearch Reason { get; set; }

        /// <summary>
        ///     When the report was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// Sort property & direction.
        /// </summary>
        public Sort<PostReportSort> Sort { get; set; }
        
        /// <summary>
        ///     Result pagination.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}