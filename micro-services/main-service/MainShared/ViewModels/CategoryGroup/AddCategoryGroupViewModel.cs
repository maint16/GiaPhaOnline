using System.ComponentModel.DataAnnotations;

namespace MainShared.ViewModels.CategoryGroup
{
    public class AddCategoryGroupViewModel
    {
        #region Properties

        /// <summary>
        ///     Name of category group
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Description of category group
        /// </summary>
        [Required]
        public string Description { get; set; }

        #endregion
    }
}