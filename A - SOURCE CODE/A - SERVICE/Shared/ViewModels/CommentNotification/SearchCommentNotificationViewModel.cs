using System;
using System.Collections.Generic;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using Shared.Models;

namespace Shared.ViewModels.CommentNotification
{
    public class SearchCommentNotificationViewModel
    {
        #region Properties

        /// <summary>
        /// Id of notification.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Id of comment.
        /// </summary>
        public int? CommentId { get; set; }

        /// <summary>
        /// Id of post which comment belongs to.
        /// </summary>
        public int? PostId { get; set; }
        
        /// <summary>
        /// Type of notification.
        /// </summary>
        public HashSet<NotificationType> Types { get; set; }

        /// <summary>
        /// Statuses of notification.
        /// </summary>
        public HashSet<NotificationStatus> Statuses { get; set; }
        
        /// <summary>
        /// Time when the notification was created.
        /// </summary>
        public Range<double?, double?> CreatedTime { get; set; }

        /// <summary>
        /// Sort property & direction.
        /// </summary>
        public Sort<CommentNotificationSort> Sort { get; set; }

        /// <summary>
        /// Pagination information.
        /// </summary>
        public Pagination Pagination { get; set; }

        #endregion
    }
}