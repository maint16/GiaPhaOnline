using System.ComponentModel.DataAnnotations;

namespace AppShared.ViewModels.Reply
{
    public class EditReplyViewModel
    {
        #region Properties

        /// <summary>
        ///     Content of reply
        /// </summary>
        [Required]
        public string Content { get; set; }

        #endregion
    }
}