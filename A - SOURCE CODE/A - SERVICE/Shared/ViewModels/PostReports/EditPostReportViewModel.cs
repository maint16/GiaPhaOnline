using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.PostReports
{
    public class EditPostReportViewModel
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