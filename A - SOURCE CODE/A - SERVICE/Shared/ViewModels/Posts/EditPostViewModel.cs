using SystemConstant.Enumerations;

namespace Shared.ViewModels.Posts
{
    public class EditPostViewModel
    {
        #region Properties

        /// <summary>
        /// Title of post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Body of post.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Type of post.
        /// </summary>
        public PostType? Type { get; set; }

        #endregion
    }
}