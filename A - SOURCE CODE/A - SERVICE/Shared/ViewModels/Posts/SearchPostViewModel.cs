using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Posts
{
    public class SearchPostViewModel
    {
        #region Properties

        /// <summary>
        ///     Post index.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Id of post owner.
        /// </summary>
        public int? OwnerId { get; set; }
        
        /// <summary>
        ///     Title search option.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Body of post search.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Statuses of post.
        /// </summary>
        public HashSet<PostStatus> Statuses { get; set; }

        /// <summary>
        /// Types of post.
        /// </summary>
        public HashSet<PostType> Types { get; set; }

        /// <summary>
        ///     When the post was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        ///     When the post was lastly modified.
        /// </summary>
        public Range<double?, double?> LastModifiedTime { get; set; }

        /// <summary>
        /// Sort property & direction.
        /// </summary>
        public Sort<PostSort> Sort { get; set; }
        
        /// <summary>
        ///     Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}