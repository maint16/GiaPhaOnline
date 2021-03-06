﻿using System.ComponentModel.DataAnnotations;

namespace MainShared.ViewModels.Users
{
    public class ChangePasswordViewModel
    {
        #region Properties

        /// <summary>
        ///     Old password
        /// </summary>
        [Required]
        public string OriginalPassword { get; set; }

        /// <summary>
        ///     New password
        /// </summary>
        [Required]
        public string Password { get; set; }

        #endregion
    }
}