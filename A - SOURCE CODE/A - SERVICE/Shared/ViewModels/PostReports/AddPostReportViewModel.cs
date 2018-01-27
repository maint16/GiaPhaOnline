using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.PostReports
{
    public class AddPostReportViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of post which should be reported.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Reason why the post should be reported.
        /// </summary>
        [Required]
        public string Reason { get; set; }

        #endregion
    }
}