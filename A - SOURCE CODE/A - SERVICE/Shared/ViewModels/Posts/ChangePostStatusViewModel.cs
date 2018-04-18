using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SystemConstant.Enumerations;

namespace Shared.ViewModels.Posts
{
    public class ChangePostStatusViewModel
    {
        #region Properties

        /// <summary>
        /// Reason of change.
        /// </summary>
        [Required]
        public string Reason { get; set; }

        /// <summary>
        /// Status of post.
        /// </summary>
        public PostStatus Status { get; set; }

        #endregion
    }
}
