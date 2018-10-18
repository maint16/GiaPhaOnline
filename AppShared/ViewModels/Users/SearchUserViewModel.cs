using System.Collections.Generic;
using ClientShared.Enumerations;
using ClientShared.Enumerations.Order;
using ClientShared.Models;

namespace AppShared.ViewModels.Users
{
    public class SearchUserViewModel
    {
        #region Properties

        /// <summary>
        ///     List of account ids
        /// </summary>
        public HashSet<int> Ids { get; set; }

        /// <summary>
        ///     List of account emails
        /// </summary>
        public HashSet<string> Emails { get; set; }

        /// <summary>
        ///     List of account statuses
        /// </summary>
        public HashSet<UserStatus> Statuses { get; set; }

        /// <summary>
        ///     List of account roles
        /// </summary>
        public HashSet<UserRole> Roles { get; set; }

        /// <summary>
        ///     Pagination
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        ///     Sort property & direction
        /// </summary>
        public Sort<UserSort> Sort { get; set; }

        #endregion
    }
}