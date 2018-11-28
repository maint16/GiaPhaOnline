using System.Collections.Generic;

namespace MainBusiness.Models.NotificationMessages
{
    public class AddUserGroupNotificationMessageModel<T>
    {
        #region Properties

        public T ExtraInfo { get; set; }

        public string Message { get; set; }

        public HashSet<int> IgnoredUserIds { get; set; }

        #endregion

        #region Constructor

        public AddUserGroupNotificationMessageModel()
        {
        }

        public AddUserGroupNotificationMessageModel(T extraInfo, string message)
        {
            ExtraInfo = extraInfo;
            Message = message;
        }

        public AddUserGroupNotificationMessageModel(T extraInfo, string message, HashSet<int> ignoredUserIds)
        {
            ExtraInfo = extraInfo;
            Message = message;
            IgnoredUserIds = ignoredUserIds;
        }

        #endregion
    }
}