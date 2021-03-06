﻿using System.ComponentModel.DataAnnotations;

namespace AuthenticationShared.ViewModels.User
{
    public class GoogleLoginViewModel
    {
        #region Properties

        /// <summary>
        ///     Google id token.
        ///     If this value is specified. Id token will be checked instead of exchanging code with Google server.
        /// </summary>
        [Required]
        public string IdToken { get; set; }

        #endregion
    }
}