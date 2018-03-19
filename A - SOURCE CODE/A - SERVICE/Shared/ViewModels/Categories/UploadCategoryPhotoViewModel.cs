using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Shared.ViewModels.Categories
{
    public class UploadCategoryPhotoViewModel
    {
        #region Properties

        /// <summary>
        /// Photo of Category. Should be formatted as (512x512)
        /// </summary>
        [Required]
        public IFormFile Photo { get; set; }

        #endregion
    }
}
