using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClientShared.Enumerations;
using ClientShared.Models;

namespace MainShared.ViewModels.NotificationMessage
{
    public class SearchNotificationMessageViewModel
    {
        #region Properties

        /// <summary>
        ///     Statuses of notification.
        /// </summary>
        public HashSet<NotificationStatus> Statuses { get; set; }

        /// <summary>
        ///     Range of created time.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        ///     Pagination information.
        /// </summary>
        [Required]
        public Pagination Pagination { get; set; }

        #endregion

        #region Constructor

        public SearchNotificationMessageViewModel()
        {
        }

        public SearchNotificationMessageViewModel(HashSet<NotificationStatus> statuses,
            Range<double?, double?> createdTime, Pagination pagination)
        {
            Statuses = statuses;
            CreatedTime = createdTime;
            Pagination = pagination;
        }

        #endregion
    }
}