using System.ComponentModel.DataAnnotations;
using AppModel.Enumerations;

namespace Main.ViewModels.Reply
{
    public class EditReplyViewModel
    {
        #region Properties

        //    /// <summary>
        //    ///   Topic that reply belongs to.
        //    /// </summary>
        //    public int TopicId { get; set; }
        //
        //    /// <summary>
        //    ///     Category that reply belongs to.
        //    /// </summary>
        //    [Required]
        //    public int CategoryId { get; set; }
        //
        //    /// <summary>
        //    ///     Category group that topic belongs to.
        //    /// </summary>
        //    [Required]
        //    public int CategoryGroupId { get; set; }

        /// <summary>
        /// Content of reply
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        ///     Status of reply.
        /// </summary>
        public ItemStatus Status { get; set; }

        #endregion
    }
}
