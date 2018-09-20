using System.ComponentModel.DataAnnotations;
using AppModel.Enumerations;

namespace Main.ViewModels.Category
{
    public class EditCategoryViewModel
    {
        #region Properties

        /// <summary>
        ///     Category group that category belongs to.
        /// </summary>
        [Required]
        public int CategoryGroupId { get; set; }

        /// <summary>
        /// Name of category
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Description of category
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     Status of category.
        /// </summary>
        public ItemStatus Status { get; set; }

        #endregion
    }
}
