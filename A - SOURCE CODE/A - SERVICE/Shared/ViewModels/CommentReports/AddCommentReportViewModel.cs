
using System.ComponentModel.DataAnnotations;
using Shared.Resources;

namespace Shared.ViewModels.CommentReports
{
    public class AddCommentReportViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of comment which should be reported.
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        ///     Reason why the comment should be reported.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(HttpValidationMessages), ErrorMessageResourceName = "InformationIsRequired")]
        public string Reason { get; set; }

        #endregion
    }
}