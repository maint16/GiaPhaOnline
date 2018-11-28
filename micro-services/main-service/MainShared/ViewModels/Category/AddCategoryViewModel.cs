using System.ComponentModel.DataAnnotations;

namespace MainShared.ViewModels.Category
{
    public class AddCategoryViewModel
    {
        #region Properties

        /// <summary>
        ///     Category group that category belongs to.
        /// </summary>
        [Required]
        public int CategoryGroupId { get; set; }

        /// <summary>
        ///     Name of category
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Description of category
        /// </summary>
        [Required]
        public string Description { get; set; }

        #endregion
    }
}