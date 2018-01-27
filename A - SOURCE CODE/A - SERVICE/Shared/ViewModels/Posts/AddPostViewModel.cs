using System.ComponentModel.DataAnnotations;
using SystemConstant.Enumerations;

namespace Shared.ViewModels.Posts
{
    public class AddPostViewModel
    {
        #region Properties

        /// <summary>
        /// Title of post.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Post body.
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// Post type.
        /// </summary>
        public PostType Type { get; set; }
        
        #endregion
    }
}