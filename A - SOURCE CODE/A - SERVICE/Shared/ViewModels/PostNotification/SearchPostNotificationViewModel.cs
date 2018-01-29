using System;
using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.PostNotification
{
    public class SearchPostNotificationViewModel
    {
        #region Properties

        /// <summary>
        /// Id of notification.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Id of post.
        /// </summary>
        public int? PostId { get; set; }
        
        /// <summary>
        /// Types of notification.
        /// </summary>
        public HashSet<NotificationType> Types { get; set; }

        /// <summary>
        /// Statuses of notification.
        /// </summary>
        public HashSet<NotificationStatus> Statuses { get; set; }
        
        /// <summary>
        /// Time when notification was created
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// Sorted property & direction.
        /// </summary>
        public Sort<PostNotificationSort> Sort { get; set; }

        /// <summary>
        /// Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}