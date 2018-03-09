using System;
using System.Collections.Generic;
using System.Text;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.Accounts
{
    public class RequestPasswordViewModel
    {
        /// <summary>
        ///     Id of account.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///     Email address.
        /// </summary>
        public TextSearch Email { get; set; }

        /// <summary>
        ///     Nickname of account owner.
        /// </summary>
        public TextSearch Nickname { get; set; }

        /// <summary>
        /// Hashed password.
        /// </summary>
        public TextSearch Password { get; set; }

        /// <summary>
        ///     Account status.
        /// </summary>
        public AccountStatus[] Statuses { get; set; }

        /// <summary>
        ///     Time when account joined.
        /// </summary>
        public Range<double?, double?> JoinedTime { get; set; }

        /// <summary>
        ///     Time when account was lastly modified.
        /// </summary>
        public Range<double?, double?> LastModifiedTime { get; set; }

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
