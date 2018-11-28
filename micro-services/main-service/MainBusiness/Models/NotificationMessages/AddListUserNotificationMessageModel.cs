using System.Collections.Generic;

namespace MainBusiness.Models.NotificationMessages
{
    public class AddListUserNotificationMessageModel<T>
    {
        #region Properties

        /// <summary>
        ///     Notification message owner.
        /// </summary>
        public HashSet<int> OwnerIds { get; set; }

        public T ExtraInfo { get; set; }

        public string Message { get; set; }

        #endregion

        #region Constructor

        public AddListUserNotificationMessageModel()
        {
        }

        public AddListUserNotificationMessageModel(HashSet<int> ownerIds, T extraInfo, string message)
        {
            OwnerIds = ownerIds;
            ExtraInfo = extraInfo;
            Message = message;
        }

        #endregion
    }
}