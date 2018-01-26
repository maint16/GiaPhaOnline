using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.Comments
{
    public class EditCommentViewModel
    {
        #region Properties

        /// <summary>
        /// Comment content.
        /// </summary>
        [Required]
        public string Content { get; set; }

        #endregion
    }
}