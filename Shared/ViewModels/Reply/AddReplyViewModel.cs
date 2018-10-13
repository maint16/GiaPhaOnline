using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.Reply
{
    public class AddReplyViewModel
    {
        #region Properties

        /// <summary>
        ///     Topic that reply belongs to.
        /// </summary>
        [Required]
        public int TopicId { get; set; }

        /// <summary>
        ///     Content of reply
        /// </summary>
        [Required]
        public string Content { get; set; }

        #endregion
    }
}