using Shared.Enumerations.Order;
using Shared.Models;

namespace Shared.ViewModels.Categories
{
    public class SearchCategoryViewModel
    {
        /// <summary>
        ///     Id of category.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Id of creator.
        /// </summary>
        public int? CreatorId { get; set; }

        /// <summary>
        ///     Name of category.
        /// </summary>
        public TextSearch Name { get; set; }

        /// <summary>
        ///     When the category was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        ///     When the category was lastly modified.
        /// </summary>
        public Range<double?, double?> LastModifiedTime { get; set; }

        /// <summary>
        ///     Which property should be used for sorting categories.
        /// </summary>
        public CategoriesSort Sort { get; set; }

        /// <summary>
        ///     Whether records should be sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection Direction { get; set; }

        /// <summary>
        ///     Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}