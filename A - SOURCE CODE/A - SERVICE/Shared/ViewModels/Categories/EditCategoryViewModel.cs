using SystemConstant.Enumerations;

namespace Shared.ViewModels.Categories
{
    public class EditCategoryViewModel
    {
        #region Properties

        /// <summary>
        /// Name of category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Status of category.
        /// </summary>
        public CategoryStatus? Status { get; set; }

        /// <summary>
        /// Photo of category (base64 encoded)
        /// </summary>
        public string Photo { get; set; }

        #endregion
    }
}