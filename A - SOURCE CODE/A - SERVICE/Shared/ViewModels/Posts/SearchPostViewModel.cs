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
        ///     Id of category which post belongs to.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        ///     Title search option.
        /// </summary>
        public TextSearch Title { get; set; }

        /// <summary>
        ///     Body of post search.
        /// </summary>
        public TextSearch Body { get; set; }

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