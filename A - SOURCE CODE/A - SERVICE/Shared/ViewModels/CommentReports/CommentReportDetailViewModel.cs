using SystemDatabase.Models.Entities;

namespace Shared.ViewModels.CommentReports
{
    public class CommentReportDetailViewModel
    {
        #region Properties

        /// <summary>
        ///     Comment which is reported.
        /// </summary>
        public Comment Comment { get; set; }

        /// <summary>
        ///     The post comment belongs to.
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        ///     Owner of comment.
        /// </summary>
        public Account CommentOwner { get; set; }

        /// <summary>
        ///     Who created the report.
        /// </summary>
        public Account CommentReporter { get; set; }

        /// <summary>
        ///     Content of report.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Reason the comment is reported.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        ///     When the report was created.
        /// </summary>
        public double Created { get; set; }

        #endregion
    }
}