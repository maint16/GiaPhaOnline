﻿using System.ComponentModel.DataAnnotations;
using AppModel.Enumerations;

namespace Main.ViewModels.CategoryGroup
{
    public class EditCategoryGroupViewModel
    {
        #region Properties

        /// <summary>
        ///     Name of category group.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Description of category group.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     Status of category group.
        /// </summary>
        public ItemStatus Status { get; set; }

        #endregion
    }
}
