using System.ComponentModel.DataAnnotations;
using Shared.Enumerations;

namespace Shared.ViewModels.Reply
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