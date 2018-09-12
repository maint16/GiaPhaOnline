using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.Reply
{
    public class AddReplyViewModel
    {
        #region Properties

        /// <summary>
        ///   Topic that reply belongs to.
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        ///     Category that reply belongs to.
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        ///     Category group that topic belongs to.
        /// </summary>
        [Required]
        public int CategoryGroupId { get; set; }

        /// <summary>
        /// Content of reply
        /// </summary>
        [Required]
        public string Content { get; set; }

        #endregion
    }
}
