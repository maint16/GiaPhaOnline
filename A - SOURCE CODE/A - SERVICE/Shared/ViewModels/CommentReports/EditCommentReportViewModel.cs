using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.CommentReports
{
    public class EditCommentReportViewModel
    {
        #region Properties

        /// <summary>
        /// Reason to report a post.
        /// </summary>
        [Required]
        public string Reason { get; set; }

        #endregion
    }
}