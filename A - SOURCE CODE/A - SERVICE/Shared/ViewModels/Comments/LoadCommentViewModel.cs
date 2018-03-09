using System;
using System.Collections.Generic;
using System.Text;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Comments
{
    public class LoadCommentViewModel
    {
        /// <summary>
        /// List of comment indexes.
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// Sort property & direction.
        /// </summary>
        public Sort<CommentSort> Sort { get; set; }

        /// <summary>
        ///     Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}
