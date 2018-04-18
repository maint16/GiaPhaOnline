﻿using SystemConstant.Enumerations;
using Microsoft.AspNetCore.Http;

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
        public ItemStatus? Status { get; set; }

        ///// <summary>
        ///// Photo of category
        ///// </summary>
        //public IFormFile Photo { get; set; }

        #endregion
    }
}