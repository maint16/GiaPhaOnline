using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.Comments
{
    public class AddCommentViewModel
    {
        #region Properties

        /// <summary>
        /// Id of post.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Content of comment.
        /// </summary>
        [Required]
        public string Content { get; set; }

        #endregion
    }
}