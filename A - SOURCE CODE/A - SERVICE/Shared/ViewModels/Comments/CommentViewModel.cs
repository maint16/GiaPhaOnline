using SystemConstant.Enumerations;
using SystemDatabase.Models.Entities;

namespace Shared.ViewModels.Comments
{
    public class CommentViewModel
    {
        /// <summary>
        /// Id of comment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Owner information.
        /// </summary>
        public Account Owner { get; set; }

        /// <summary>
        /// Post information.
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        /// Content of comment.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Status of comment.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        /// Time when comment was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        /// Time when comment was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }
    }
}