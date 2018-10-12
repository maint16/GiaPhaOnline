﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Main.ViewModels.Users
{
    public class UploadPhotoViewModel
    {
        #region Properties

        /// <summary>
        /// User photo
        /// </summary>
        [Required]
        public IFormFile Photo { get; set; }

        #endregion
    }
}