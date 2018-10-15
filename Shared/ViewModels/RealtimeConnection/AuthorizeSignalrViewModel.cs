﻿using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.RealtimeConnection
{
    public class AuthorizeSignalrViewModel
    {
        #region Properties

        /// <summary>
        ///     Connection index (socket id)
        /// </summary>
        [Required]
        public string Id { get; set; }

        #endregion
    }
}