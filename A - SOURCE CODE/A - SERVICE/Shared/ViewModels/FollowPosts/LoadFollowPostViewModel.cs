using System;
using System.Collections.Generic;
using System.Text;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.FollowPosts
{
    public class LoadFollowPostViewModel
    {
        /// <summary>
        ///     Id of post.
        /// </summary>
        public List<int> PostIds { get; set; }

        /// <summary>
        /// Sorted property & direction.
        /// </summary>
        public Sort<FollowPostSort> Sort { get; set; }

        /// <summary>
        ///     Pagination of records filter.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}
