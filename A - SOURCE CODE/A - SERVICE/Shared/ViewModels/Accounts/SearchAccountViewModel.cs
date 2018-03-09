﻿using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Accounts
{
    public class SearchAccountViewModel
    {
        /// <summary>
        ///     Id of account.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Account status.
        /// </summary>
        public HashSet<AccountStatus> Statuses { get; set; }

        /// <summary>
        ///     Account role.
        /// </summary>
        public HashSet<AccountRole> Roles { get; set; }

        /// <summary>
        /// Sorted property & direction.
        /// </summary>
        public Sort<AccountSort> Sort { get; set; }
        
        /// <summary>
        ///     Pagination of records filter.
        /// </summary>
        public Pagination Pagination { get; set; }
    }
}