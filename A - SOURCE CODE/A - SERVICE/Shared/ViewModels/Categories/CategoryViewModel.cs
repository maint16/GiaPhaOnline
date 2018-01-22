using SystemConstant.Enumerations;

namespace Shared.ViewModels.Categories
{
    public class CategoryViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Who created the current category.
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        ///     Category photo.
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        ///     Status of category.
        /// </summary>
        public CategoryStatus Status { get; set; }

        /// <summary>
        ///     Name of category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     When the category was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the category was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion
    }
}