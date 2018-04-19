using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SystemConstant.Enumerations;

namespace Shared.ViewModels.Comments
{
    public class ChangeCommentStatusViewModel
    {
        #region Properties

        /// <summary>
        /// Reason of change.
        /// </summary>
        [Required]
        public string Reason { get; set; }

        /// <summary>
        /// Status of comment.
        /// </summary>
        public ItemStatus Status { get; set; }

        #endregion
    }
}
