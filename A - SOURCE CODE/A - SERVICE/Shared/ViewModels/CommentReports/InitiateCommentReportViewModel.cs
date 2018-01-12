
namespace Shared.ViewModels.CommentReports
{
    public class InitiateCommentReportViewModel
    {
        /// <summary>
        ///     Id of comment which should be reported.
        /// </summary>
        public int CommentIndex { get; set; }

        /// <summary>
        ///     Reason why the comment should be reported.
        /// </summary>
        public string Reason { get; set; }
    }
}