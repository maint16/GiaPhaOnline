using System.Collections;
using System.Collections.Generic;
using Shared.Enumerations;

namespace Main.Models
{
    public class RealTimeNotification
    {
        #region Properties

        /// <summary>
        /// Category of notification.
        /// </summary>
        public NotificationCategory Category { get; }

        /// <summary>
        /// Notification action
        /// </summary>
        public NotificationAction Action { get; }

        /// <summary>
        /// Additional data to pass to client.
        /// </summary>
        public IDictionary Data { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize real-time notification with specific parameters.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="action"></param>
        public RealTimeNotification(NotificationCategory category, NotificationAction action)
        {
            Category = category;
            Action = action;
        }

        /// <summary>
        /// Initialize real-time notification with specific parameters.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public RealTimeNotification(NotificationCategory category, NotificationAction action, IDictionary data) : this(category, action)
        {
            Data = data;
        }

        #endregion
    }
}