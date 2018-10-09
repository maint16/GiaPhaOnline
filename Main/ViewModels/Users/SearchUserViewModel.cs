﻿using System.Collections.Generic;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Models;
using Shared.Models;

namespace Main.ViewModels.Users
{
    public class SearchUserViewModel
    {
        #region Properties

        /// <summary>
        /// List of account ids
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        /// List of account emails
        /// </summary>
        public HashSet<string> Emails { get; set; }

        /// <summary>
        /// List of account statuses
        /// </summary>
        public HashSet<UserStatus> Statuses { get; set; }

        /// <summary>
        /// List of account roles
        /// </summary>
        public HashSet<UserRole> Roles { get; set; }

        /// <summary>
        /// Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        /// Sort property & direction
        /// </summary>
        public Sort<AccountSort> Sort { get; set; }

        #endregion
    }
}
