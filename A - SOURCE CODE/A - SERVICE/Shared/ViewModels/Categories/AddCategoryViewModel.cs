﻿using System.ComponentModel.DataAnnotations;
using Shared.Resources;

namespace Shared.ViewModels.Categories
{
    public class AddCategoryViewModel
    {
        #region Properties

        /// <summary>
        /// Photo of category. Should be formatted as (512x512)
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(HttpValidationMessages), ErrorMessageResourceName = "InformationIsRequired")]
        public string Photo { get; set; }

        /// <summary>
        /// Name of category.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(HttpValidationMessages), ErrorMessageResourceName = "InformationIsRequired")]
        public string Name { get; set; }

        #endregion
    }
}