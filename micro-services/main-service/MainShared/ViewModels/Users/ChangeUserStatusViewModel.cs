﻿using ClientShared.Enumerations;

namespace MainShared.ViewModels.Users
{
    public class ChangeUserStatusViewModel
    {
        #region Properties

        /// <summary>
        ///     Account status in the system.
        /// </summary>
        public UserStatus Status { get; set; }

        #endregion
    }
}